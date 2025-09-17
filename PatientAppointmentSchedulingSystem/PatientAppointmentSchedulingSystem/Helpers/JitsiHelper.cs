namespace PatientAppointmentSchedulingSystem.Helpers
{
    public class JitsiHelper
    {
        public static string GenerateMeetingLink(string topic)
        {
            string roomName = $"{topic}-{Guid.NewGuid()}";
            return $"https://meet.jit.si/{roomName}";
        }
    }
}
