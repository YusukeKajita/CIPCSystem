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
			BASE(int myPort, const char * remoteIP = "127.0.0.1", int remotePort = 2000, int fps = 60)
				:mode(MODE::non),
				myport(myPort),
				remoteIP(remoteIP),
				remotePort(remotePort),
				fps(fps)
			{
				#ifdef windows_user
				WSADATA wsadata;
				if (0 != WSAStartup(MAKEWORD(2, 0), &wsadata))
				{
					printf("WSAStartup Faild.\n");
				}
				#endif
				#ifdef DEBUG
				std::cout << "[info][cipc_base]開始。" << std::endl;
				#endif
			}

			virtual ~BASE()
			{
				#ifdef DEBUG
				std::cout << "[info][cipc_base]終了。" << std::endl;
				#endif
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

			//modeがSenderならSend,ReceiverならReceiveを実行。
			void Update(std::vector<unsigned char>& data)
			{
				this->_Update(data);
				this->Update_add(data);
			}

			//接続終了前に入れること。これによってconnect_addで切断処理を実行することができる。
			void Close()
			{
				this->udp_client->Close();
				this->udp_client.release();
				Sleep(100);
				this->Close_add();
			}

			//
			CIPC::CLIENT::MODE getCurrentMode() const {
				return this->mode;
			}
			int getCurrentReceivedCheck() const {
				return this->receivedlength;
			}
		protected:
			//method
			//Updateで実行される関数。Senderのとき実行される。dataheaderを追加するときはここに追記する。
			virtual void Send(std::vector<unsigned char>& data)
			{
				#ifdef DEBUG
				std::cout << "[info][send]送信。" << std::endl;
				#endif
				this->udp_client->Send(data);
			}
			//Updateで実行される関数。Receiverのとき実行される。dataheaderを追加するときはここに追記する。
			virtual void Receive(std::vector<unsigned char>& data)
			{
				#ifdef DEBUG
				std::cout << "[info][receive]受信開始。" << std::endl;
				#endif
				data = this->udp_client->Receive(&receivedlength);
				#ifdef DEBUG
				std::cout << "[info][receive]受信完了。" << std::endl;
				#endif
			}
			//Setupで実行される関数。Senderの接続確認および初期通信に使用
			virtual void Connect_Sender_add()
			{
				#ifdef DEBUG
				std::cout << "[info][connect_sender_add]仮想関数。" << std::endl;
				#endif
			}
			//Setupで実行される関数。Receiverの接続確認および初期通信に使用
			virtual void Connect_Receiver_add()
			{
				#ifdef DEBUG
				std::cout << "[info][connect_receiver_add]仮想関数。" << std::endl;
				#endif
			}
			//Setupで実行される関数。初期通信に使用。
			virtual void Connect_add()
			{
				#ifdef DEBUG
				std::cout << "[info][connect_add]仮想関数。" << std::endl;
				#endif
			}
			//Updateで実行される関数。予備
			virtual void Update_add(std::vector<unsigned char>& data)
			{
				#ifdef DEBUG
				std::cout << "[info][uppdate_add]仮想関数。" << std::endl;
				#endif
			}
			//Close(destructer)で実行される関数。
			virtual void Close_add()
			{
				#ifdef DEBUG
				std::cout << "[info][close_add]仮想関数。" << std::endl;
				#endif
			}

			//field
			MODE mode;
			std::auto_ptr< UDP_PACKETS_CODER::UDP_PACKETS_CLIENT> udp_client;
			UDP_PACKETS_CODER::UDP_PACKETS_ENCODER enc;
			UDP_PACKETS_CODER::UDP_PACKETS_DECODER dec;

			//自ポート
			unsigned int myport;
			int receivedlength;
			int fps;
			//相手ポート　ReceiveModeのときは指定しなくていい
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