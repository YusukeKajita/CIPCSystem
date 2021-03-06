#pragma once

namespace UDP_PACKETS_CODER
{
	//あらゆる型のデータをunsigned charの配列に変換します
	template<typename Type>
	union data_changer
	{
		struct{
			Type _data;
		};
		struct
		{
			unsigned char uchar_data[sizeof(Type)];
		};
	};

	//char型をunsigned char, unsigned char型をchar型に変えます
	union CharanduChar
	{
		char char_data;
		unsigned char uchar_data;
	};

	//配列の長さを取得します
	template<typename TYPE, std::size_t SIZE>
	std::size_t array_length(const TYPE (&)[SIZE])
	{
		return SIZE;
	}
}