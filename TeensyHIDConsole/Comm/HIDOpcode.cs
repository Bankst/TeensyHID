namespace TeensyHID.Comm
{
    public enum HIDOpcode
    {
        NULL,
        MESSAGE_ACK,
        INIT,
        INIT_ACK,
        HEARTBEAT,
        HEARTBEAT_ACK,
        LOOP_DATA,
        LOOP_DATA_ACK,

    }
}
