using UnityEngine;

namespace GiveawaySystems.Scripts
{
    [System.Serializable]
    public class ImageConstraints
    {
        public Vector2 maxSize = new Vector2(512, 512);
        public Vector2 minSize = new Vector2(64, 64);
        public float aspectRatioTolerance = 0.1f;
        public bool maintainAspectRatio = true;
        public float targetAspectRatio = 1.0f; // 1:1 by default

        public bool ValidateSize(Vector2 size)
        {
            return size.x >= minSize.x && size.x <= maxSize.x &&
                   size.y >= minSize.y && size.y <= maxSize.y;
        }

        public bool ValidateAspectRatio(Vector2 size)
        {
            if (!maintainAspectRatio) return true;
            
            float currentAspectRatio = size.x / size.y;
            float minAspectRatio = targetAspectRatio - aspectRatioTolerance;
            float maxAspectRatio = targetAspectRatio + aspectRatioTolerance;
            
            return currentAspectRatio >= minAspectRatio && currentAspectRatio <= maxAspectRatio;
        }
    }

    public class ImageConstraintManager : MonoBehaviour
    {
        public ImageConstraints pcConstraints;
        public ImageConstraints mobileConstraints;

        public bool ValidateImage(Texture2D texture, bool isMobile = false)
        {
            if (texture == null) return false;

            var constraints = isMobile ? mobileConstraints : pcConstraints;
            Vector2 size = new Vector2(texture.width, texture.height);

            bool sizeValid = constraints.ValidateSize(size);
            bool aspectValid = constraints.ValidateAspectRatio(size);

            return sizeValid && aspectValid;
        }

        public Vector2 GetTargetSize(Texture2D texture, bool isMobile = false)
        {
            var constraints = isMobile ? mobileConstraints : pcConstraints;
            Vector2 originalSize = new Vector2(texture.width, texture.height);

            if (constraints.ValidateSize(originalSize) && constraints.ValidateAspectRatio(originalSize))
            {
                return originalSize;
            }

            // Calculate target size while maintaining aspect ratio
            float aspectRatio = originalSize.x / originalSize.y;
            Vector2 targetSize = originalSize;

            // Scale down if larger than max size
            if (targetSize.x > constraints.maxSize.x)
            {
                targetSize.x = constraints.maxSize.x;
                targetSize.y = targetSize.x / aspectRatio;
            }
            if (targetSize.y > constraints.maxSize.y)
            {
                targetSize.y = constraints.maxSize.y;
                targetSize.x = targetSize.y * aspectRatio;
            }

            // Scale up if smaller than min size
            if (targetSize.x < constraints.minSize.x)
            {
                targetSize.x = constraints.minSize.x;
                targetSize.y = targetSize.x / aspectRatio;
            }
            if (targetSize.y < constraints.minSize.y)
            {
                targetSize.y = constraints.minSize.y;
                targetSize.x = targetSize.y * aspectRatio;
            }

            return targetSize;
        }

        public Texture2D ResizeImage(Texture2D originalTexture, bool isMobile = false)
        {
            Vector2 targetSize = GetTargetSize(originalTexture, isMobile);
            
            // If no resize needed, return original
            if (targetSize.x == originalTexture.width && targetSize.y == originalTexture.height)
            {
                return originalTexture;
            }

            // Create new texture with target size
            Texture2D resizedTexture = new Texture2D((int)targetSize.x, (int)targetSize.y);
            float xScale = targetSize.x / originalTexture.width;
            float yScale = targetSize.y / originalTexture.height;

            for (int y = 0; y < targetSize.y; y++)
            {
                for (int x = 0; x < targetSize.x; x++)
                {
                    // Sample original texture (simple point sampling)
                    int sourceX = Mathf.FloorToInt(x / xScale);
                    int sourceY = Mathf.FloorToInt(y / yScale);
                    Color pixel = originalTexture.GetPixel(sourceX, sourceY);
                    resizedTexture.SetPixel(x, y, pixel);
                }
            }

            resizedTexture.Apply();
            return resizedTexture;
        }
    }
}
