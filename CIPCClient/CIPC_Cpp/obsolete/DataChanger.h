#pragma once

namespace UDP_PACKETS_CODER
{
	//������^�̃f�[�^��unsigned char�̔z��ɕϊ����܂�
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

	//char�^��unsigned char, unsigned char�^��char�^�ɕς��܂�
	union CharanduChar
	{
		char char_data;
		unsigned char uchar_data;
	};

	//�z��̒������擾���܂�
	template<typename TYPE, std::size_t SIZE>
	std::size_t array_length(const TYPE (&)[SIZE])
	{
		return SIZE;
	}
}