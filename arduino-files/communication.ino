#include <LiquidCrystal.h>

String inputString = "";
String commandString = "";
boolean stringComplete = false;
boolean isConnected = false;

LiquidCrystal lcd (12, 11, 5, 4, 3, 2);

void setup() {  
  Serial.begin(9600);
  initDisplay();
}

void loop() {

if(stringComplete)
{
  stringComplete = false;
  getCommand();
  
  if(commandString.equals("SYSCON")) { //SYSTEM CONNECT
    lcd.clear();
  }

  if(commandString.equals("SYSDSC")) { //SYSTEM DISCONNECT
    lcd.clear();
    lcd.print("Ready to connect");    
  }
  else if(commandString.equals("SYSDSP")) { //SYSTEM DISPLAY
    String text = getTextToPrint();
    printText(text);
  }
  
  inputString = "";
}

}

void initDisplay()
{
  lcd.begin(16, 2);
  printText("Waiting for connection...");
}

void getCommand()
{
  if(inputString.length() > 0) {
     commandString = inputString.substring(1,7);
  }
}

String getTextToPrint()
{
  String value = inputString.substring(7,inputString.length()-1);
  return value;
}

void printText(String text)
{
  lcd.clear();
  lcd.setCursor(0,0);

  if(text.length()<16) {
    lcd.print(text);
  }
  else {
    lcd.print(text.substring(0,16));
    lcd.setCursor(0,1);
    lcd.print(text.substring(16,32));
  }
}

void serialEvent() {
  while (Serial.available()) {
    char inputChar = (char)Serial.read();
    inputString += inputChar;

    if (inputChar == '\n') {
      stringComplete = true;
    }
  }
}
