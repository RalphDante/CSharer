namespace CSharer.Core.Models 
{ 	
    public class VideoPost 	{     	
        public string VideoPath { get; set; } = string.Empty;     	
        public string FileName { get; set; } = string.Empty;     	
        public string GeneratedCaption { get; set; } = string.Empty;     	
        public DateTime DetectedAt { get; set; }     	
        public bool PostedToBuffer { get; set; }     	
        public bool PostedToPinterest { get; set; } 	
    } 
}