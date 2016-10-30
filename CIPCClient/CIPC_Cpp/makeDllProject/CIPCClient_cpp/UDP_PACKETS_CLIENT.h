
#pragma once

#define windows_user
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#ifdef windows_user
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")
#endif
#ifndef windows_user
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#endif


namespace UDP_PACKETS_CODER
{

	enum class MODE
	{
		SEND,
		RECEIVE,
		NOTINITIALIZED
	};

	class UDP_PACKETS_CLIENT
	{
	public:
		/*constructer*/
		//reciever mode
		UDP_PACKETS_CLIENT(unsigned int port)
			:b_connect(false)
		{
			try{
				memset(&sockAddrIn, 0, sizeof(sockAddrIn));
				sockAddrIn.sin_port = htons(port);
				sockAddrIn.sin_family = AF_INET;
				sockAddrIn.sin_addr.s_addr = htonl(INADDR_ANY);
				mode = MODE::RECEIVE;
				
				remotesocksize = sizeof(remotesock);

				mysock = socket(AF_INET, SOCK_DGRAM, 0);
				if (INVALID_SOCKET == mysock)
				{
					throw new std::string("error from socket");
				}
				if (SOCKET_ERROR == ::bind(mysock, (const struct sockaddr *) &sockAddrIn, sizeof(sockAddrIn)))
				{
					closesocket(mysock);
					return;
				}
				isClosed = false;
			}
			catch (std::exception ex){
				throw;
			}
		}
		//sender mode
		UDP_PACKETS_CLIENT(unsigned int port, const char * remoteIP, unsigned int remotePort)
			:b_connect(true)
		{
			try{
				memset(&remotesock, 0, sizeof(remotesock));
				remotesock.sin_port = htons(remotePort);
				remotesock.sin_family = AF_INET;
				remotesock.sin_addr.s_addr = inet_addr(remoteIP);

				memset(&sockAddrIn, 0, sizeof(sockAddrIn));
				sockAddrIn.sin_port = htons(port);
				sockAddrIn.sin_family = AF_INET;
				sockAddrIn.sin_addr.s_addr = htonl(INADDR_ANY);

				remotesocksize = sizeof(remotesock);

				mode = MODE::SEND;

				mysock = socket(AF_INET, SOCK_DGRAM, 0);
				if (INVALID_SOCKET == mysock)
				{
					throw new std::string("error from socket");
				}
				if (SOCKET_ERROR == ::bind(mysock, (const struct sockaddr *) &sockAddrIn, sizeof(sockAddrIn)))
				{
					closesocket(mysock);
					return;
				}
				isClosed = false;
			}
			catch (std::exception ex){
				throw;
			}
		}

		/*destructer*/
		virtual ~UDP_PACKETS_CLIENT()
		{
			if (isClosed == false) {
				this->Close();
			}
		}

		//receive data
		std::vector<unsigned char> Receive()
		{
			try{
				CharanduChar cauc;
				char _receiveBuffer[65507];
				memset(_receiveBuffer, 0, sizeof(_receiveBuffer));
				int check = recvfrom(mysock, _receiveBuffer, 65507, 0, (sockaddr*)&remotesock, &remotesocksize);
				if (check != SOCKET_ERROR)
				{
					if (this->mode == MODE::RECEIVE)
					{
						this->mode = MODE::SEND;
					}
					this->data.clear();
					for (int t = 0; t < check; t++)
					{
						cauc.char_data = _receiveBuffer[t];
						this->data.push_back(cauc.uchar_data);
					}
					this->data.shrink_to_fit();
					return this->data;
				}

				return this->data;
			}
			catch (std::exception ex){
				throw;
			}
		}

		void Send(std::vector<unsigned char> data)
		{
			try{
				std::vector<char> _data;
				for (auto &it : data)
				{
					CharanduChar cauc;
					cauc.uchar_data = it;
					_data.push_back(cauc.char_data);
				}
				sendto(mysock, &_data.front(), _data.size(), 0, (sockaddr*)&remotesock, sizeof(remotesock));
			}
			catch (std::exception ex){
				throw;
			}
		}

		void Send(const char* data, int length)
		{
			try {
				
				sendto(mysock, data, length, 0, (sockaddr*)&remotesock, sizeof(remotesock));
			}
			catch (std::exception ex) {
				throw;
			}
		}

		void Close()
		{
			try{
				closesocket(this->mysock);
				this->isClosed = true;
			}
			catch (std::exception ex){
				throw;
			}
		}

	private:
		MODE mode;
		unsigned short port;
		SOCKET mysock;
		sockaddr_in sockAddrIn;
		sockaddr_in remotesock;
		int remotesocksize;
		bool b_connect;
		bool isClosed;

		std::vector<unsigned char> data;
	};
}