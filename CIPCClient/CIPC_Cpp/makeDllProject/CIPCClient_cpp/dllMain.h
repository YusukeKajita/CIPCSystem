#pragma once

#ifdef DLLCIPCClient_EXPORTS
#define DLLCIPCClient_API extern "C" _declspec(dllexport)
#else
#define DLLCIPCClient_API extern "C" _declspec(dllimport)
#endif

DLLCIPCClient_API void cipc_initialize();


DLLCIPCClient_API int cipc_client_add(char* ipaddress, int port, int myport, int fps);

DLLCIPCClient_API void cipc_connect(int id, int isSender);

DLLCIPCClient_API void cipc_update(int id);

DLLCIPCClient_API void cipc_push_back_double(int id, double value);
DLLCIPCClient_API void cipc_push_back_float(int id, float value);
DLLCIPCClient_API void cipc_push_back_int(int id, int value);
DLLCIPCClient_API void cipc_reset_sender_buffer(int id);

DLLCIPCClient_API double cipc_pop_front_double(int id);
DLLCIPCClient_API float cipc_pop_front_float(int id);
DLLCIPCClient_API int cipc_pop_front_int(int id);
DLLCIPCClient_API void cipc_reset_receiver_buffer(int id);


DLLCIPCClient_API void cipc_disconnect(int id);
