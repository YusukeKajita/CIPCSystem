
#include "../../CIPC_MAIN.h"
#include <Windows.h>
#include <memory>
#define DLL_MAKE extern "C" __declspec(dllexport)
using namespace std;
shared_ptr<CIPC::CLIENT::MAIN> client;
std::vector<unsigned char> _data;
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	switch (fdwReason)
	{
	case    DLL_PROCESS_ATTACH:
		break;
	case    DLL_PROCESS_DETACH:
		break;
	case    DLL_THREAD_ATTACH:
		break;
	case    DLL_THREAD_DETACH:
		break;
	}
	return  TRUE;
}

DLL_MAKE int Setup(int ServerPort, int myPort, char* remoteIPAddress, int fps, int connectmode) {
	client.reset(new CIPC::CLIENT::MAIN(myPort, remoteIPAddress, ServerPort, fps));
	switch (connectmode)
	{
	case 2:
		client->Setup(CIPC::CLIENT::Sender);
		break;
	case 3:
		client->Setup(CIPC::CLIENT::Receiver);
		break;
	default:
		return -1;
	}
	return 0;
}

DLL_MAKE char* Update(char* data) {
	if (client->getCurrentMode() == CIPC::CLIENT::Sender) {
		std::vector<unsigned char> _data;
		for (int i = 0; '\0' != *(data+i); i++)
		{
			_data.push_back(*(data+i));
		}
		client->Update(_data);
		return "Success";
	}
	if (client->getCurrentMode() == CIPC::CLIENT::Receiver) {
		int length = 0;
		
		client->Update(_data);
		_data.at(client->getCurrentReceivedCheck()) = '\0';
		return (char*)_data.data();
	}
}

DLL_MAKE void Close() {
	client->Close();
}