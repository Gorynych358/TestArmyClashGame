using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle
{
    /// <summary>
    /// Генератор пастельных цветов и контрастных пастельных цветов.
    /// </summary>
    public static class RandomPastelColorGenerator
    {
        /// <summary>
        /// Генерирует случайный пастельный цвет.
        /// </summary>
        public static Color GeneratePastelColor(
            float minSaturation = 0.25f,
            float maxSaturation = 0.45f,
            float minValue = 0.75f,
            float maxValue = 0.9f)
        {
            Color color;

            do
            {
                float hue = Random.Range(0f, 1f);
                float saturation = Random.Range(minSaturation, maxSaturation);
                float value = Random.Range(minValue, maxValue);

                color = Color.HSVToRGB(hue, saturation, value);
            }
            while (IsTooGray(color));

            return color;
        }

        /// <summary>
        /// Генерирует пастельный цвет, контрастный к исходному.
        /// Контраст достигается выбором противоположного оттенка (Hue + 180°).
        /// </summary>
        /// <param name="sourceColor">Исходный цвет, от которого нужно получить контрастный.</param>
        public static Color GenerateContrastColor(Color sourceColor)
        {
            // Получаем Hue исходного цвета
            Color.RGBToHSV(sourceColor, out float sourceHue, out _, out _);

            // Противоположный оттенок (Hue + 180°)
            float oppositeHue = sourceHue + 0.5f;
            if (oppositeHue > 1f)
                oppositeHue -= 1f;

            // Добавляем небольшой разброс ±0.08 (≈ ±30°)
            float hue = Random.Range(oppositeHue - 0.08f, oppositeHue + 0.08f);
            hue = Mathf.Repeat(hue, 1f);

            // Пастельные параметры
            float saturation = Random.Range(0.25f, 0.45f);
            float value = Random.Range(0.75f, 0.9f);

            Color result = Color.HSVToRGB(hue, saturation, value);

            // Проверка на серость
            if (IsTooGray(result))
                return GenerateContrastColor(sourceColor);

            return result;
        }

        /// <summary>
        /// Проверяем на серость, и отсекаем слишком серые тона.
        /// </summary>
        private static bool IsTooGray(Color color)
        {
            float maxComponent = Mathf.Max(color.r, color.g, color.b);
            float minComponent = Mathf.Min(color.r, color.g, color.b);
            return (maxComponent - minComponent) < 0.2f;
        }
    }
}