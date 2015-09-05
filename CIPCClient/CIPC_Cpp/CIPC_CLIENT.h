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

#ifdef DEBUG
				std::cout << "[info][cipc]�J�n�B" << std::endl;
#endif
			}

			MAIN(int myPort, char * remoteIP, int remotePort)
				:BASE(myPort, remoteIP, remotePort)
			{
#ifdef DEBUG
				std::cout << "[info][cipc]�J�n�B����I�����[�h" << std::endl;
#endif
				this->serverPort = remotePort;
			}

			virtual ~MAIN()
			{

#ifdef DEBUG
				std::cout << "[info][cipc]�I���B" << std::endl;
#endif
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
						throw new std::exception("�ڑ����s");
					}
					this->udp_client->Close();
					this->udp_client.release();
					Sleep(100);
					this->remotePort = dec.get_data<int>();
#ifdef DEBUG
					std::cout << "[info][cipc]�ڑ������B����U��ꂽ�����[�g�|�[�g�F" << this->remotePort << std::endl;
#endif
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
					this->enc << SETTINGS::myinfo::fps;
					this->enc << SETTINGS::CONNECTION_COMANDS::MODE_SEND;
					this->enc << (int)SETTINGS::myinfo::name_length * 2;
					for (int t = 0; t < SETTINGS::myinfo::name_length; t++)
					{
						this->enc << SETTINGS::myinfo::name[t];
					}

					this->udp_client->Send(this->enc.get_vectordata());
					dec.set_data(this->udp_client->Receive());
#ifdef DEBUG
					std::cout << "[info][cipc]�ڑ������B�����F" << this->dec.get_data<int>() << std::endl;
#endif
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
					this->enc << SETTINGS::myinfo::fps;
					this->enc << SETTINGS::CONNECTION_COMANDS::MODE_RECEIVE;
					this->enc << (int)SETTINGS::myinfo::name_length * 2;
					for (int t = 0; t < SETTINGS::myinfo::name_length; t++)
					{
						this->enc << SETTINGS::myinfo::name[t];
					}

					this->udp_client->Send(this->enc.get_vectordata());
					dec.set_data(this->udp_client->Receive());
#ifdef DEBUG
					std::cout << "[info][cipc]�ڑ������B�����F" << this->dec.get_data<int>() << std::endl;
#endif
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
#ifdef DEBUG
						std::cout << "[info][cipc]�ؒf�����B" << std::endl;
#endif
					}
					else
					{
#ifdef DEBUG
						std::cout << "[info][cipc]�ؒf���s�B" << std::endl;
#endif
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