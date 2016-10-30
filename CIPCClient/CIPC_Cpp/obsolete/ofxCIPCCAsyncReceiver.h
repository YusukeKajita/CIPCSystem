#pragma once

#include "CIPC_MAIN.h"
#include "ofMain.h"

namespace CIPC
{
	namespace OF
	{
		class AsyncReceiver : public ofThread
		{
		public:
			AsyncReceiver(int myport, char * remoteip, int remoteport)
				:cipc(myport,remoteip,remoteport)
			{
				cipc.Setup(CIPC::CLIENT::MODE::Receiver);
			}

			virtual ~AsyncReceiver()
			{
			}

			void threadedFunction(){
				while(this->isThreadRunning()){
					try{
						cipc.Update(this->data);
					}catch(std::exception ex)
					{
						std::cout<<ex.what()<<std::endl;
					}
				}
			}
			std::vector<unsigned char> data;
			CIPC::CLIENT::MAIN cipc;
		};
	}
}
