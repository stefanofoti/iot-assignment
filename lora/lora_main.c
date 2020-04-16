#include <string.h>
#include <inttypes.h>

#include "msg.h"
#include "shell.h"
#include "fmt.h"

#include "net/loramac.h"
#include "semtech_loramac.h"

semtech_loramac_t loramac;

int main(void){
    semtech_loramac_init(&loramac);

    //DEVEUI, APPKEY and APPEUI come from the makefile or as command line aurguments while making the .elf 

    char DR[] = "5";

    //set deveui
    puts("Setting deveui...");
    uint8_t deveui[LORAMAC_DEVEUI_LEN];
    fmt_hex_bytes(deveui, DEVEUI);
    semtech_loramac_set_deveui(&loramac, deveui);

    //set appeui
    puts("Setting appeui...");
    uint8_t appeui[LORAMAC_APPEUI_LEN];
    fmt_hex_bytes(appeui, APPEUI);
    semtech_loramac_set_appeui(&loramac, appeui);

    //set appkey
    puts("Setting app key...");
    uint8_t appkey[LORAMAC_APPKEY_LEN];
    fmt_hex_bytes(appkey, APPKEY);
    semtech_loramac_set_appkey(&loramac, appkey);

    //set dr
    puts("Setting dr...");
    uint8_t dr = atoi(DR);
    semtech_loramac_set_dr(&loramac, dr);

    //OTAA join 
    puts("Joining...");
    uint8_t join_type = LORAMAC_JOIN_OTAA;
    switch (semtech_loramac_join(&loramac, join_type)) {
        case SEMTECH_LORAMAC_DUTYCYCLE_RESTRICTED:
            puts("Cannot join: dutycycle restriction");
            return 1;
        case SEMTECH_LORAMAC_BUSY:
            puts("Cannot join: mac is busy");
            return 1;
        case SEMTECH_LORAMAC_JOIN_FAILED:
            puts("Join procedure failed!");
            return 1;
        case SEMTECH_LORAMAC_ALREADY_JOINED:
            puts("Warning: already joined!");
            return 1;
        case SEMTECH_LORAMAC_JOIN_SUCCEEDED:
        	//It should go here
            puts("Join procedure succeeded!");
            break;
        default:
            puts("ERROR: Unable to join");
            return 1;
    }

    puts("Start sending...");
    uint8_t cnf = LORAMAC_DEFAULT_TX_MODE;  /* Default: confirmable */
    uint8_t port = LORAMAC_DEFAULT_TX_PORT; /* Default: 2 */

    //Start sending
    char msg[192];
    int index=0;
    int temp;
    int humidity;
    int wind1;
    int wind2;
    int rain;

    while(true){
        puts("Sending message...");
        index=index+1;
        temp=(rand() % 100)-50;
        humidity=rand() % 100;
        wind1=rand() % 360;
        wind2=rand() % 100;
        rain=rand() % 50;
        snprintf(msg, sizeof(msg), "{\"messageId\":%d,\"temperature\":%d,\"humidity\":%d,\"windDirection\":%d,\"windIntensity\":%d,\"rain\":%d}", index, temp, humidity, wind1, wind2, rain);

        semtech_loramac_set_tx_mode(&loramac, cnf);
        semtech_loramac_set_tx_port(&loramac, port);

        switch (semtech_loramac_send(&loramac, (uint8_t *)msg, strlen(msg))) {
            case SEMTECH_LORAMAC_NOT_JOINED:
                puts("Cannot send: not joined");
                return 1;

            case SEMTECH_LORAMAC_DUTYCYCLE_RESTRICTED:
                puts("Cannot send: dutycycle restriction");
                return 1;

            case SEMTECH_LORAMAC_BUSY:
                puts("Cannot send: MAC is busy");
                return 1;

            case SEMTECH_LORAMAC_TX_ERROR:
                puts("Cannot send: error");
                return 1;
        }

        puts("Wait for receive windows...");
        /* wait for receive windows */
        switch (semtech_loramac_recv(&loramac)) {
            case SEMTECH_LORAMAC_DATA_RECEIVED:
                loramac.rx_data.payload[loramac.rx_data.payload_len] = 0;
                printf("Data received: %s, port: %d\n",
                       (char *)loramac.rx_data.payload, loramac.rx_data.port);
                break;

            case SEMTECH_LORAMAC_DUTYCYCLE_RESTRICTED:
                puts("Cannot send: dutycycle restriction");
                return 1;

            case SEMTECH_LORAMAC_BUSY:
                puts("Cannot send: MAC is busy");
                return 1;

            case SEMTECH_LORAMAC_TX_ERROR:
                puts("Cannot send: error");
                return 1;

            case SEMTECH_LORAMAC_TX_DONE:
                puts("TX complete, no data received");
                break;
        }

        if (loramac.link_chk.available) {
            printf("Link check information:\n"
                   "  - Demodulation margin: %d\n"
                   "  - Number of gateways: %d\n",
                   loramac.link_chk.demod_margin,
                   loramac.link_chk.nb_gateways);
        }
        //Sleep for 60 sec before sending a new message
        xtimer_sleep(60);
    }
    return 0;
}
