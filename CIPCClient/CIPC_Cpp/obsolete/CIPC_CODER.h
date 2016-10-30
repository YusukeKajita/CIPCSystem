#pragma once
#include <vector>

namespace CIPC
{
	namespace CODER
	{
		class MHCODER
		{
		public:
			//MH•ÏŠ·
			//”’•‰æ‘œ‚È‚Ç‚ğintŒ^‚Ì”z—ñ‚É•ÏŠ·‚µ‚Ü‚·B”’‚U•‚Q”’‚VË6,2,7
			static std::vector<int> MHENCODER(std::vector<bool> vec_bool)
			{
				try{
					std::vector<int> vec_int;
					bool oldstate = false;
					int count = 0;
					for (int i = 0; i < (int)vec_bool.size(); i++)
					{
						if (vec_bool.at(i) != oldstate)
						{
							int t = count;
							vec_int.push_back(t);
							count = 0;
						}
						++count;
						oldstate = vec_bool.at(i);
					}
					vec_int.push_back(count);
					return vec_int;
				}
				catch (std::exception ex){
					throw;
				}
			}
			//MH‹t•ÏŠ·
			static std::vector<bool> MHDECODER(std::vector<int> vec_int)
			{
				try{
					std::vector<bool> vec_bool;
					for (int i = 0; i < (int)vec_int.size(); i++)
					{
						for (int k = 0; k < vec_int.at(i); k++)
						{
							if (i % 2)
							{
								vec_bool.push_back(false);
							}
							else
							{
								vec_bool.push_back(true);
							}
						}
					}
					return vec_bool;
				}
				catch (std::exception ex){
					throw;
				}
			}
		};
	}
}