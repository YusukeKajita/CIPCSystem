#pragma once
/*****************UDP_PACKETS_CODER***************************************************/
/*通信用パケットを作成・解読するクラスです。使用する標準データは、unsigned charです。*/
/*encoder で演算子を利用してデータを追加し、ゲッタで出来上がったunsigned char データを回収します*/
/*decoder でセッタおよびコンストラクタでデータをセットしゲッタでデータを取得します。*/
/************************************************************************************/

#include "UDP_PACKETS_DECODER.h"
#include "UDP_PACKETS_ENCODER.h"
#include "UDP_PACKETS_CLIENT.h"