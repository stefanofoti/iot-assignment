import random
import time

from azure.iot.device import IoTHubDeviceClient, Message

# The device connection string to authenticate the device with your IoT hub.
# Get it using the Azure CLI:
# az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyNodeDevice --output table
CONNECTION_STRING_1 = ""
CONNECTION_STRING_2 = ""

def sendToAzure(topic, message):
    try:
        client1 = IoTHubDeviceClient.create_from_connection_string(CONNECTION_STRING_1)
        client2 = IoTHubDeviceClient.create_from_connection_string(CONNECTION_STRING_2)
        print( "Sending message: {}".format(message))
        message = message.encode('utf8')
        if topic=="sensors/s1":
            print("Sending to S1")
            client1.send_message(message)        
        if topic=="sensors/s2":
            print("Sending to S2")
            client2.send_message(message)
        print ( "Message successfully sent" )        
    except KeyboardInterrupt:
        print ( "IoTHubBridge sample stopped" )

