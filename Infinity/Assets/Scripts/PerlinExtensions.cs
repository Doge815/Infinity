using System;
using TreeEditor;

namespace Assets.Scripts
{
    public static class PerlinExtensions
    {
        private static float ApplyOctaves(Func<float, float> getScaled, int octaves, float octaveAmplitude, float octaveScale)
        {
            var aggregate = getScaled(1);

            var amplitude = 1f;
            var scale = 1f;

            for (var i = 0; i < octaves; i++)
            {
                amplitude *= octaveAmplitude;
                scale *= octaveScale;
                aggregate += amplitude * getScaled(scale);
            }

            return aggregate;
        }

        public static float NoiseWithOctaves(this Perlin perlin, int octaves, float octaveAmplitude, float octaveScale, float arg) =>
            ApplyOctaves(s => perlin.Noise(s * arg), octaves, octaveAmplitude, octaveScale);

        public static float NoiseWithOctaves(this Perlin perlin, int octaves, float octaveAmplitude, float octaveScale, float x, float y) =>
            ApplyOctaves(s => perlin.Noise(s * x, s * y), octaves, octaveAmplitude, octaveScale);

        public static float NoiseWithOctaves(this Perlin perlin, int octaves, float octaveAmplitude, float octaveScale, float x, float y, float z) =>
            ApplyOctaves(s => perlin.Noise(s * x, s * y, s * z), octaves, octaveAmplitude, octaveScale);
    }
}