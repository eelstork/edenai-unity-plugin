using System;

public static class ImageUtils{
    
    public static string EncodeImageToBase64(string imagePath)
    {
        if (!System.IO.File.Exists(imagePath))
            throw new System.IO.FileNotFoundException($"Image not found: {imagePath}");

        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(imageBytes);
    }
}
