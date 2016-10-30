#include "CIPC_SETTINGS.h"

using namespace CIPC;

//�����h�o�A�h���X�̐ݒ�B
const char* SETTINGS::RemoteExchangerServer::IP = "192.168.11.10";
//�����CIPS�̐���|�[�g
const int SETTINGS::RemoteExchangerServer::Port = 12000;
//�������g�p����|�[�g
const int SETTINGS::Port::myPort = 4009;

//������client�̖��O
const wchar_t SETTINGS::myinfo::name[] = L"TEAWASE";
const int SETTINGS::myinfo::name_length = sizeof(SETTINGS::myinfo::name) / sizeof(SETTINGS::myinfo::name[0]);
const int SETTINGS::myinfo::fps = 0;

//�f�t�H���g�̃��[�h�ݒ�B���M���[�h�ł̓f�[�^�𑗐M����f�q�A��M���[�h�ł̓f�[�^����M����f�q�����p����B
const CLIENT::MODE SETTINGS::MODE::Mode = CLIENT::MODE::Sender;



//connection comands �o�[�W�����Œ�̒l
const int SETTINGS::CONNECTION_COMANDS::DEMANDS = 1;
const int SETTINGS::CONNECTION_COMANDS::END = 9;
const int SETTINGS::CONNECTION_COMANDS::MODE_SEND = 2;
const int SETTINGS::CONNECTION_COMANDS::MODE_RECEIVE = 3;
const int SETTINGS::CONNECTION_COMANDS::OK = 0;