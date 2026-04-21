using UnityEngine;

namespace ACT.Scripts
{  
    /// <summary>
    /// Генератор случайных цветов в постельных тонах, чтобы были не "вырвиглазные" тона, 
    /// и отсекаем слишком серые тона.
    /// Параметры генерации можно настроить, можно оставить по-умалчанию.
    /// </summary>
    public static class RandomPastelColorGenerator
    {
        /// <summary>
        /// Генерирует случайный пастельный цвет с контролем насыщенности и светлоты.
        /// </summary>
        /// <param name="minSaturation">Минимальная насыщенность (0.0–1.0)</param>
        /// <param name="maxSaturation">Максимальная насыщенность (0.0–1.0)</param>
        /// <param name="minLightness">Минимальная светлота (0.0–1.0)</param>
        /// <param name="maxLightness">Максимальная светлота (0.0–1.0)</param>
        /// <returns>Пастельный цвет</returns>
        public static Color GeneratePastelColor(
            float minSaturation = 0.15f,
            float maxSaturation = 0.4f,
            float minLightness = 0.7f,
            float maxLightness = 0.93f)
        {
            Color color;
            do
            {
                float hue = Random.Range(0f, 360f);
                float saturation = Random.Range(minSaturation, maxSaturation);
                float lightness = Random.Range(minLightness, maxLightness);
                color = Color.HSVToRGB(hue / 360f, saturation, lightness);
            }
            while (IsTooGray(color));

            return color;
        }

        /// <summary>
        /// Генерирует пастельный цвет для команды: холодные тона для защитников, тёплые — для захватчиков.
        /// </summary>
        /// <param name="isDefender">true — защитник (холодные тона), false — захватчик (тёплые тона)</param>
        /// <param name="minSaturation">Минимальная насыщенность</param>
        /// <param name="maxSaturation">Максимальная насыщенность</param>
        /// <param name="minLightness">Минимальная светлота</param>
        /// <param name="maxLightness">Максимальная светлота</param>
        /// <returns>Пастельный цвет команды</returns>
        public static Color GenerateTeamColor(
            bool isDefender,
            float minSaturation = 0.25f,
            float maxSaturation = 0.45f,
            float minLightness = 0.75f,
            float maxLightness = 0.9f)
        {
            float hue;

            if (isDefender)
            {
                // Холодные тона: синий, голубой, фиолетовый (180–300°)
                hue = Random.Range(180f, 300f);
            }
            else
            {
                // Тёплые тона: красный, оранжевый, розовый
                // Два диапазона для лучшего распределения
                if (Random.value < 0.5f)
                {
                    hue = Random.Range(0f, 60f); // Красный/оранжевый
                }
                else
                {
                    hue = Random.Range(300f, 360f); // Розовый/пурпурный
                }
            }

            float saturation = Random.Range(minSaturation, maxSaturation);
            float lightness = Random.Range(minLightness, maxLightness);

            Color color = Color.HSVToRGB(hue / 360f, saturation, lightness);

            // Дополнительная проверка на слишком серые цвета
            if (IsTooGray(color))
            {
                // Если цвет слишком серый, генерируем заново с небольшими корректировками
                return GenerateTeamColor(isDefender, minSaturation, maxSaturation, minLightness, maxLightness);
            }

            return color;
        }

        /// <summary>
        /// Проверяет, является ли цвет слишком серым (низкий контраст между компонентами).
        /// </summary>
        /// <param name="color">Проверяемый цвет</param>
        /// <returns>true, если цвет слишком серый</returns>
        private static bool IsTooGray(Color color)
        {
            // Вычисляем разницу между максимальным и минимальным компонентами RGB
            float maxComponent = Mathf.Max(color.r, color.g, color.b);
            float minComponent = Mathf.Min(color.r, color.g, color.b);
            float colorDifference = maxComponent - minComponent;

            // Если разница меньше порога — цвет воспринимается как серый
            const float grayThreshold = 0.2f;
            return colorDifference < grayThreshold;
        }
    }
}