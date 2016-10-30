#include "dllmain.h"
#include <memory>
#include <vector>
#include "CIPC_MAIN.h"
#include <unordered_map>

using namespace std;
shared_ptr<unordered_map<int, shared_ptr<CIPC::CLIENT::MAIN> > > cipc_clients;
shared_ptr<unordered_map<int, shared_ptr<UDP_PACKETS_CODER::UDP_PACKETS_DECODER> > > cipc_decorders;
shared_ptr<unordered_map<int, shared_ptr<UDP_PACKETS_CODER::UDP_PACKETS_ENCODER> > > cipc_encorders;

void cipc_send(int id);
void cipc_receive(int id);


void cipc_initialize()
{
	cipc_clients = make_shared<unordered_map<int, shared_ptr<CIPC::CLIENT::MAIN> > >();
	cipc_encorders = make_shared<unordered_map<int, shared_ptr<UDP_PACKETS_CODER::UDP_PACKETS_ENCODER> > >();
	cipc_decorders = make_shared<unordered_map<int, shared_ptr<UDP_PACKETS_CODER::UDP_PACKETS_DECODER> > >();
}

int cipc_client_add(char * ipaddress, int port, int myport, int fps)
{
	int id = cipc_clients->size();
	try
	{
		cipc_clients->insert(pair<int, shared_ptr<CIPC::CLIENT::MAIN> >(id, make_shared<CIPC::CLIENT::MAIN>(myport, ipaddress, port, fps)));
		cipc_decorders->insert(pair<int, shared_ptr<UDP_PACKETS_CODER::UDP_PACKETS_DECODER>>(id, make_shared<UDP_PACKETS_CODER::UDP_PACKETS_DECODER>()));
		cipc_encorders->insert(pair<int, shared_ptr<UDP_PACKETS_CODER::UDP_PACKETS_ENCODER>>(id, make_shared<UDP_PACKETS_CODER::UDP_PACKETS_ENCODER>()));
	}
	catch (std::exception ex)
	{
		return -1;
	}
	return id;
}



void cipc_connect(int id, int isSender)
{
	try
	{

		switch (isSender)
		{
		case 0:
			cipc_clients->at(id)->Setup(CIPC::CLIENT::Sender);
			break;
		case 1:
			cipc_clients->at(id)->Setup(CIPC::CLIENT::Receiver);
			break;
		default:
			return;
			break;
		}
	}
	catch (std::exception ex)
	{
	}
}

void cipc_update(int id)
{
	switch (cipc_clients->at(id)->GetMode())
	{
	case CIPC::CLIENT::MODE::Sender:
		cipc_send(id);
		break;
	case CIPC::CLIENT::MODE::Receiver:
		cipc_receive(id);
		break;
	default:
		return;
		break;
	}
}

void cipc_push_back_double(int id, double value)
{
	cipc_encorders->at(id)->addData(value);
}

void cipc_push_back_float(int id, float value)
{
	cipc_encorders->at(id)->addData(value);
}

void cipc_push_back_int(int id, int value)
{
	cipc_encorders->at(id)->addData(value);
}

void cipc_reset_sender_buffer(int id)
{
	cipc_encorders->at(id)->reset();
}

double cipc_pop_front_double(int id)
{
	return cipc_decorders->at(id)->get_data<double>();
}

float cipc_pop_front_float(int id)
{
	return cipc_decorders->at(id)->get_data<float>();
}

int cipc_pop_front_int(int id)
{
	return cipc_decorders->at(id)->get_data<int>();
}

double cipc_push_back_double(int id)
{
	return cipc_decorders->at(id)->get_data<double>();
}

float cipc_push_back_float(int id)
{
	return cipc_decorders->at(id)->get_data<float>();
}

int cipc_push_back_int(int id)
{
	return cipc_decorders->at(id)->get_data<int>();
}

void cipc_reset_receiver_buffer(int id)
{
	cipc_decorders->at(id)->reset();
}

void cipc_disconnect(int id)
{
	cipc_clients->at(id)->Close();
}

void cipc_send(int id)
{
	vector<unsigned char> data = cipc_encorders->at(id)->get_vectordata();
	cipc_clients->at(id)->Update(data);
}

void cipc_receive(int id)
{
	vector<unsigned char> data;
	cipc_clients->at(id)->Update(data);
	cipc_decorders->at(id)->set_data(data);
}


