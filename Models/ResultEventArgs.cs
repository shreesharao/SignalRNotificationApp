namespace Terarecon.Eureka.Cardiac.NotificationService.Models
{
    class ResultEventArgs
    {
          public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string StudyInstanceUid { get; set; }
        public string StudyDescription { get; set; }
        public string SeriesInstanceUid { get; set; }
        public int SeriesNumber { get; set; }
        public string SopInstanceUid { get; set; }
    }
}