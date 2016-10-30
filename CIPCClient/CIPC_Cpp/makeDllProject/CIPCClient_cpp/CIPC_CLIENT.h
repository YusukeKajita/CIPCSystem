#pragma once

#include "CIPC_CLIENT_BASE.h"
#include "CIPC_SETTINGS.h"


namespace CIPC
{
	namespace CLIENT
	{
		class MAIN : public BASE
		{
		public:
			MAIN()
				:BASE(SETTINGS::Port::myPort, SETTINGS::RemoteExchangerServer::IP, SETTINGS::RemoteExchangerServer::Port)
			{
			}

			MAIN(int myPort, char * remoteIP, int remotePort)
				:BASE(myPort, remoteIP, remotePort)
			{
				this->serverPort = remotePort;
			}

			MAIN(int myPort, char * remoteIP, int remotePort,int fps)
				:BASE(myPort, remoteIP, remotePort, fps)
			{
				this->serverPort = remotePort;
			}

			virtual ~MAIN()
			{
				this->Close();
			}
		private:
			void Connect_add()
			{
				try{
					this->enc.reset();
					this->enc << SETTINGS::CONNECTION_COMANDS::DEMANDS;
					this->udp_client->Send(enc.get_vectordata());
					dec.set_data(this->udp_client->Receive());
					if (this->dec.get_data<int>() != SETTINGS::CONNECTION_COMANDS::OK)
					{
						throw new std::string("Ú‘±Ž¸”s");
					}
					this->udp_client->Close();
					this->udp_client.release();
					Sleep(100);
					this->remotePort = dec.get_data<int>();
				}
				catch (std::exception ex){
					throw;
				}

			}

			void Connect_Sender_add()
			{
				try{
					this->enc.reset();
					this->enc << SETTINGS::CONNECTION_COMANDS::DEMANDS;
					this->enc << this->FPS;
					this->enc << SETTINGS::CONNECTION_COMANDS::MODE_SEND;
					this->enc << (int)SETTINGS::myinfo::name_length * 2;
					for (int t = 0; t < SETTINGS::myinfo::name_length; t++)
					{
						this->enc << SETTINGS::myinfo::name[t];
					}

					this->udp_client->Send(this->enc.get_vectordata());
					dec.set_data(this->udp_client->Receive());
				}
				catch (std::exception ex){
					throw;
				}
			}

			void Connect_Receiver_add()
			{
				try{
					this->enc.reset();
					this->enc << SETTINGS::CONNECTION_COMANDS::DEMANDS;
					this->enc << this->FPS;
					this->enc << SETTINGS::CONNECTION_COMANDS::MODE_RECEIVE;
					this->enc << (int)SETTINGS::myinfo::name_length * 2;
					for (int t = 0; t < SETTINGS::myinfo::name_length; t++)
					{
						this->enc << SETTINGS::myinfo::name[t];
					}

					this->udp_client->Send(this->enc.get_vectordata());
					dec.set_data(this->udp_client->Receive());
				}
				catch (std::exception ex){
					throw;
				}
			}
			
			void Close_add()
			{
				try{
					udp_client.reset(new UDP_PACKETS_CODER::UDP_PACKETS_CLIENT(this->myport, this->remoteIP, this->serverPort));


					this->enc.reset();
					this->enc << SETTINGS::CONNECTION_COMANDS::END;
					this->enc << this->remotePort;
					this->udp_client->Send(this->enc.get_vectordata());

					
					dec.set_data(this->udp_client->Receive());
					if (dec.get_data<int>() == SETTINGS::CONNECTION_COMANDS::END)
					{
					}
					else
					{
					}
					this->udp_client->Close();
					this->udp_client.release();
				}
				catch (std::exception ex){
					throw;
				}
			}
		};
	}
}