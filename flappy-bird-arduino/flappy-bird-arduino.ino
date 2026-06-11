#include <AudioAnalyzer.h>
//Version 1.3 for Spectrum analyzer
//Please download the lastest library file

Analyzer Audio = Analyzer(4,5,5);//Strobe pin ->4  RST pin ->5 Analog Pin ->5
//Analyzer Audio = Analyzer();//Strobe->4 RST->5 Analog->0

int FreqVal[7];//

void setup()
{
  Serial.begin(57600);
  Audio.Init();//Init module 
}

void loop()
{
  Audio.ReadFreq(FreqVal);//Return 7 values of 7 bands pass filiter
  for(int i=0;i<7;i++)            
  {
    Serial.print(max((FreqVal[i]-100),0));
    if(i<6)  Serial.print(",");
    else Serial.println();
  }
  delay(20);
}
