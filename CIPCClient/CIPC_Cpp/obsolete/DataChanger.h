#pragma once

namespace UDP_PACKETS_CODER
{
	//‚ ‚ç‚ä‚éŒ^‚Ìƒf[ƒ^‚ğunsigned char‚Ì”z—ñ‚É•ÏŠ·‚µ‚Ü‚·
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

	//charŒ^‚ğunsigned char, unsigned charŒ^‚ğcharŒ^‚É•Ï‚¦‚Ü‚·
	union CharanduChar
	{
		char char_data;
		unsigned char uchar_data;
	};

	//”z—ñ‚Ì’·‚³‚ğæ“¾‚µ‚Ü‚·
	template<typename TYPE, std::size_t SIZE>
	std::size_t array_length(const TYPE (&)[SIZE])
	{
		return SIZE;
	}
}