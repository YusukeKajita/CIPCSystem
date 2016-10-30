#pragma once
#include "UDP_PACKETS_CODER.h"
#include <memory>
#include <iostream>

//#define DEBUG

//CentralInterProcessCommunication 
namespace CIPC
{
	namespace CLIENT
	{
		enum MODE{
			Sender,
			Receiver,
			non
		};

		class BASE
		{
		public:
			BASE(int myPort, const char * remoteIP = "127.0.0.1", int remotePort = 2000)
				:mode(MODE::non),
				myport(0),
				remoteIP(remoteIP),
				remotePort(0),
				FPS(0)
			{
				#ifdef windows_user
				WSADATA wsadata;
				if (0 != WSAStartup(MAKEWORD(2, 0), &wsadata))
				{
					printf("WSAStartup Faild.\n");
				}
				#endif

				this->myport = myPort;
				this->remotePort = remotePort;
			}

			BASE(int myPort, const char * remoteIP, int remotePort, int fps)
				:mode(MODE::non),
				myport(0),
				remoteIP(remoteIP),
				remotePort(0),
				FPS(0)
			{
#ifdef windows_user
				WSADATA wsadata;
				if (0 != WSAStartup(MAKEWORD(2, 0), &wsadata))
				{
					printf("WSAStartup Faild.\n");
				}
#endif

				this->myport = myPort;
				this->remotePort = remotePort;
				this->FPS = fps;
			}

			virtual ~BASE()
			{
				#ifdef windows_user
				WSACleanup();
				#endif
			}

			void Setup(MODE mode)
			{
				this->udp_client.reset(new UDP_PACKETS_CODER::UDP_PACKETS_CLIENT(this->myport, this->remoteIP, this->remotePort));
				this->mode = mode;
				this->Connect();
			}

			MODE GetMode() 
			{
				return this->mode;
			}

			//mode��Sender�Ȃ�Send,Receiver�Ȃ�Receive�����s�B
			void Update(std::vector<unsigned char>& data)
			{
				this->_Update(data);
				this->Update_add(data);
			}

			//�ڑ��I���O�ɓ���邱�ƁB����ɂ����connect_add�Őؒf���������s���邱�Ƃ��ł���B
			void Close()
			{
				this->udp_client->Close();
				this->udp_client.release();
				Sleep(100);
				this->Close_add();
			}
		protected:
			//method
			//Update�Ŏ��s�����֐��BSender�̂Ƃ����s�����Bdataheader��ǉ�����Ƃ��͂����ɒǋL����B
			virtual void Send(std::vector<unsigned char>& data)
			{
				this->udp_client->Send(data);
			}
			//Update�Ŏ��s�����֐��BReceiver�̂Ƃ����s�����Bdataheader��ǉ�����Ƃ��͂����ɒǋL����B
			virtual void Receive(std::vector<unsigned char>& data)
			{
				this->udp_client->Send("a", 1);
				data = this->udp_client->Receive();
			}
			//Setup�Ŏ��s�����֐��BSender�̐ڑ��m�F����я����ʐM�Ɏg�p
			virtual void Connect_Sender_add()
			{
			}
			//Setup�Ŏ��s�����֐��BReceiver�̐ڑ��m�F����я����ʐM�Ɏg�p
			virtual void Connect_Receiver_add()
			{
			}
			//Setup�Ŏ��s�����֐��B�����ʐM�Ɏg�p�B
			virtual void Connect_add()
			{
			}
			//Update�Ŏ��s�����֐��B�\��
			virtual void Update_add(std::vector<unsigned char>& data)
			{
			}
			//Close(destructer)�Ŏ��s�����֐��B
			virtual void Close_add()
			{
			}

			//field
			MODE mode;
			std::auto_ptr< UDP_PACKETS_CODER::UDP_PACKETS_CLIENT> udp_client;
			UDP_PACKETS_CODER::UDP_PACKETS_ENCODER enc;
			UDP_PACKETS_CODER::UDP_PACKETS_DECODER dec;

			//���|�[�g
			unsigned int myport;
			int FPS;
			//����|�[�g�@ReceiveMode�̂Ƃ��͎w�肵�Ȃ��Ă���
			const char * remoteIP;
			unsigned int remotePort;
			unsigned int serverPort;
		private:
			//method
			void _Update(std::vector<unsigned char>& data)
			{
				switch (mode)
				{
				case MODE::non:
					try{
						this->Connect();
					}
					catch (std::exception ex)
					{
						throw std::exception(ex.what());
					}
					break;
				case MODE::Sender:
					try{
						this->Send(data);
					}
					catch (std::exception ex)
					{
						throw std::exception(ex.what());
					}
					break;
				case MODE::Receiver:
					try{
						this->Receive(data);
					}
					catch (std::exception ex)
					{
						throw std::exception(ex.what());
					}
					break;
				default:
					throw std::exception("fatal error.");
					break;
				}
			}

			void Connect()
			{
				this->Connect_add();
				switch (this->mode)
				{
				case MODE::non:
					this->mode = MODE::Sender;
					this->Connect();
					break;
				case MODE::Sender:
					this->udp_client.reset( new UDP_PACKETS_CODER::UDP_PACKETS_CLIENT(this->myport, this->remoteIP, this->remotePort));
					this->Connect_Sender_add();
					break;
				case MODE::Receiver:
					this->udp_client.reset( new UDP_PACKETS_CODER::UDP_PACKETS_CLIENT(this->myport, this->remoteIP, this->remotePort));
					this->Connect_Receiver_add();
					break;
				default:
					throw std::exception("fatal error.");
					break;
				}
			}
		};
	}
}