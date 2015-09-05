#pragma once
#include <list>
#include <vector>
#include "DataChanger.h"

//�f�[�^�̓]���p�Bunsigned char���ł̖ʓ|�Ȑl����
typedef unsigned char DATA;

//���c�ȊO�ύX���Ă͂Ȃ�Ȃ� �e���v���[�g�̂��ߎ����񕪗�
namespace UDP_PACKETS_CODER
{
	class UDP_PACKETS_DECODER
	{
	public:

		#pragma region fields
		std::vector<unsigned char> vec_data;
		int bitindex;
		#pragma endregion

		#pragma region setter and getter
		//list<unsigned char>�^�̃f�[�^���Z�b�g���܂��B
		void set_data(const std::list<unsigned char>& data)
		{
			try{
				this->vec_data.clear();
				this->bitindex = 0;
				for (auto it : data)
				{
					this->vec_data.push_back(it);
				}
			}
			catch (std::exception ex){
				throw;
			}
		}
		//vector<unsigned char>�^�̃f�[�^���Z�b�g���܂��B
		void set_data(const std::vector<unsigned char>& data)
		{
			try{
				this->vec_data.clear();
				this->bitindex = 0;
				for (auto it : data)
				{
					this->vec_data.push_back(it);
				}
			}
			catch (std::exception ex){
				throw;
			}
		}
		#pragma endregion

		#pragma region default construcer and destructer

		UDP_PACKETS_DECODER()
			:bitindex(0)
		{
		}
		UDP_PACKETS_DECODER(std::list<unsigned char>& data)
			:bitindex(0)
		{
			try{
				for (auto it : data)
				{
					this->vec_data.push_back(it);
				}
			}
			catch (std::exception ex){
				throw;
			}
		}
		UDP_PACKETS_DECODER(std::vector<unsigned char>& data)
			:bitindex(0)
		{
			try{
				for (auto it : data)
				{
					this->vec_data.push_back(it);
				}
			}
			catch (std::exception ex){
				throw;
			}
		}

		~UDP_PACKETS_DECODER()
		{
			try{
				auto it = this->vec_data.begin();
			}
			catch (std::exception ex){
				throw;
			}
		}

		#pragma endregion

		
		#pragma region public methods
		template <typename Type>
		Type get_data()
		{
			try{
				data_changer<Type> dc;

				auto it = this->vec_data.begin();

				for (int i = 0; i < sizeof(Type); i++)
				{
					dc.uchar_data[i] = this->vec_data[i + bitindex];

				}

				bitindex += sizeof(Type);
				return dc._data;
			}
			catch (std::exception ex){
				throw;
			}
		}
		#pragma endregion
	};
}