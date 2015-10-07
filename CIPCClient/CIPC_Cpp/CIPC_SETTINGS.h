#pragma once

#include "CIPC_CLIENT_BASE.h"

namespace CIPC
{
	class SETTINGS
	{
	public:
		struct RemoteExchangerServer{
			const static char* IP;
			const static int Port;
		};
		struct myinfo
		{
			const static wchar_t name[];
			const static int name_length;
			const static int fps;
		};
		struct Port
		{
			const static int myPort;
		};
		struct MODE
		{
			const static CIPC::CLIENT::MODE Mode;
		};

		struct CONNECTION_COMANDS
		{
			const static int DEMANDS;
			const static int END;
			const static int MODE_SEND;
			const static int MODE_RECEIVE;
			const static int OK;
		};
	};
}