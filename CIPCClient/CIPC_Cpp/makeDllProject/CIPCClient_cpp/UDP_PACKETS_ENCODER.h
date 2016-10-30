#pragma once
#include <list>
#include <vector>

//梶田以外変更してはならない テンプレートのため実装非分離
namespace UDP_PACKETS_CODER
{
	class UDP_PACKETS_ENCODER
	{
	public:

		#pragma region fields
		
		#pragma endregion

		#pragma region private constructer
		template <typename Type>
		UDP_PACKETS_ENCODER(UDP_PACKETS_ENCODER enc, Type add)
		{
			try{
				this->lst_data = enc.lst_data;
				data_changer<Type> dc;
				dc._data = add;
				this->ByteDataAdditioner(dc.uchar_data, sizeof(Type));
			}
			catch (std::exception ex){
				throw;
			}
		}

		#pragma endregion

		#pragma region setter and getter
		//作られたデータを取得する。list型
		std::list < unsigned char> get_listdata()
		{
			return this->lst_data;
		}
		//vector型
		std::vector<unsigned char> get_vectordata()
		{
			try{
				std::vector<unsigned char> data;
				for (auto it : this->lst_data)
				{
					data.push_back(it);
				}
				return data;
			}
			catch (std::exception ex){
				throw;
			}
		}
		//
		std::vector<char> get_vectordata_char()
		{
			try{
				std::vector<char> data;
				for (auto it : this->lst_data)
				{
					CharanduChar cauc;
					cauc.uchar_data = it;
					data.push_back(cauc.char_data);
				}
			}
			catch (std::exception ex){
				throw;
			}
		}
		#pragma endregion

		#pragma region default construcer and destructer

		UDP_PACKETS_ENCODER()
		{
		}

		~UDP_PACKETS_ENCODER()
		{
		}

		#pragma endregion
		

		#pragma region public methods
		//data resetter
		void reset()
		{
			try{
				this->lst_data.clear();

#ifdef DEBUG
				std::cout << "[info][enc] データをリセットしました。" << std::endl;
#endif
			}
			catch (std::exception ex){
				throw;
			}
		}
		#pragma endregion


		#pragma region overloads
		template <typename Type>
		UDP_PACKETS_ENCODER friend operator+(UDP_PACKETS_ENCODER enc, Type add)
		{
			return UDP_PACKETS_ENCODER(enc, add);
		}
		template <typename Type>
		void friend operator<<(UDP_PACKETS_ENCODER& enc, Type add)
		{
			enc = UDP_PACKETS_ENCODER(enc, add);
		}
		template <typename Type>
		void friend operator+=(UDP_PACKETS_ENCODER& enc, Type add)
		{
			enc = UDP_PACKETS_ENCODER(enc, add);
		}
		template <typename Type>
		void addData(Type data) 
		{
			(*this) << data;
		}
		
		#pragma endregion
	private:
		std::list<unsigned char> lst_data;
		#pragma region private method
		void ByteDataAdditioner(unsigned char * add_Data, int length)
		{
			try{
				for (int t = 0; t < length; t++){
					this->lst_data.push_back(*(add_Data + t));
				}
			}
			catch (std::exception ex)
			{
				throw;
			}
		}

		#pragma endregion 
	};
}