using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameClient.App.Utils;

internal static class NinePatchUtils
{
    internal enum NinePatchMethod
    {
        Simple,
        Repeating,
        Stretching
    }

    internal static Texture2D MakePatches(
        GraphicsDevice gd, Texture2D texture, NinePatchMethod method,
        int frameCount, int singleWidth, int singleHeight
    )
    {
        if (method != NinePatchMethod.Repeating)
        {
            throw new NotImplementedException();
        }
        var frames = GetFrames(texture, frameCount); // TODO Make changes so that the private methods all use the SetRange extension method, where it can be used
        Color[][] patchedFrames = new Color[frameCount][];

        for (int i = 0; i < frameCount; i++)
        {
            patchedFrames[i] = RepeatingNinePatch(frames[i], texture.Width, texture.Height / frameCount, singleWidth, singleHeight);
        }
        return MergeFrames(gd, patchedFrames, singleWidth, singleHeight);
    }

    static Color[][] GetFrames(Texture2D texture, int frameCount)
    {
        Color[][] frames = new Color[frameCount][];
        Color[] rawData = new Color[texture.Width * texture.Height];
        texture.GetData(rawData);
        for (int i = 0; i < frameCount; i++)
        {
            frames[i] = rawData[(rawData.Length / frameCount * i)..(rawData.Length / frameCount * (i + 1))];
        }
        return frames;
    }

    static Texture2D MergeFrames(GraphicsDevice gd, Color[][] frames, int width, int height)
    {
        Texture2D result = new(gd, width, height * frames.Length);
        Color[] rawResult = new Color[result.Width * result.Height];
        for (int i = 0; i < frames.Length; i++)
        {
            rawResult.SetRange(frames[i], frames[i].Length * i);
        }
        result.SetData(rawResult);

        return result;
    }

    static Color[] RepeatingNinePatch(
        Color[] rawData, int originalWidth, int originalHeight, int width, int height
    )
    {
        if (originalWidth % 2 != 1 || originalHeight % 2 != 1) // TODO Make a SimpleNinePatch method which accepts a Texture2D object and has a middle patch width/height of 1 pixel
        {
            throw new InvalidOperationException("Central proportions must be 1 pixel wide/tall,"
                + " the others must match in width/heigth with the opposite side.");
        }
        int horizontalTertile1 = originalWidth / 2;
        int horizontalTertile2 = originalWidth / 2 + 1;
        int verticalTertile1 = originalHeight / 2;
        int verticalTertile2 = originalHeight / 2 + 1;
        Color[][][] proportions = InitiatePatchArrays(verticalTertile1, verticalTertile2, originalHeight);

        for (int i = 0; i < verticalTertile1; i++)
        {
            proportions[0][i] = rawData[(originalWidth * i)..(originalWidth * i + horizontalTertile1)];
            proportions[1][i] = rawData[(originalWidth * i + horizontalTertile1)..(originalWidth * i + horizontalTertile2)];
            proportions[2][i] = rawData[(originalWidth * i + horizontalTertile2)..(originalWidth * (i + 1))];
        }

        for (int i = verticalTertile1; i < verticalTertile2; i++)
        {
            proportions[3][i - verticalTertile1] = rawData[(originalWidth * i)..(originalWidth * i + horizontalTertile1)];
            proportions[4][i - verticalTertile1] = rawData[(originalWidth * i + horizontalTertile1)..(originalWidth * i + horizontalTertile2)];
            proportions[5][i - verticalTertile1] = rawData[(originalWidth * i + horizontalTertile2)..(originalWidth * (i + 1))];
        }

        for (int i = verticalTertile2; i < originalHeight; i++)
        {
            proportions[6][i - verticalTertile2] = rawData[(originalWidth * i)..(originalWidth * i + horizontalTertile1)];
            proportions[7][i - verticalTertile2] = rawData[(originalWidth * i + horizontalTertile1)..(originalWidth * i + horizontalTertile2)];
            proportions[8][i - verticalTertile2] = rawData[(originalWidth * i + horizontalTertile2)..(originalWidth * (i + 1))];
        }

        var repeatingPatchArrays = CreateRepeatingPatchArrays(
            proportions, width - horizontalTertile1 - (originalWidth - horizontalTertile2),
            height - verticalTertile1 - (originalHeight - verticalTertile2)
            );
        
        var mergedArray = MergePatchArrays(repeatingPatchArrays, width, height);

        return mergedArray;
    }

    static Color[][][] InitiatePatchArrays(int verticalTertile1, int verticalTertile2, int originalHeight)
    {
        Color[][][] proportions = new Color[9][][];
        proportions[0] = new Color[verticalTertile1][];
        proportions[1] = new Color[verticalTertile1][];
        proportions[2] = new Color[verticalTertile1][];
        proportions[3] = new Color[verticalTertile2 - verticalTertile1][];
        proportions[4] = new Color[verticalTertile2 - verticalTertile1][];
        proportions[5] = new Color[verticalTertile2 - verticalTertile1][];
        proportions[6] = new Color[originalHeight - verticalTertile2][];
        proportions[7] = new Color[originalHeight - verticalTertile2][];
        proportions[8] = new Color[originalHeight - verticalTertile2][];
        return proportions;
    }

    static Color[][][] CreateRepeatingPatchArrays(Color[][][] originalPatches, int newMiddlePatchesWidth, int newMiddlePatchesHeight)
    {
        Color[][][] transformedPatches = new Color[9][][];
        transformedPatches[0] = originalPatches[0];
        transformedPatches[2] = originalPatches[2];
        transformedPatches[6] = originalPatches[6];
        transformedPatches[8] = originalPatches[8];

        int originalUpperPatchesHeigth = originalPatches[0].Length;
        int originalMiddlePatchesHeight = originalPatches[3].Length;
        int originalLowerPatchesHeight = originalPatches[6].Length;
        int originalLeftPatchesWidth = originalPatches[0][0].Length;
        int originalMiddlePatchesWidth = originalPatches[1][0].Length;
        int originalRightPatchesWidth = originalPatches[2][0].Length;

        Color[][] transformedUpperMiddlePatch = new Color[originalUpperPatchesHeigth][];
        Color[][] transformedMiddleLeftPatch = new Color[newMiddlePatchesHeight][];
        Color[][] transformedCentralPatch = new Color[newMiddlePatchesHeight][];
        Color[][] transformedMiddleRightPatch = new Color[newMiddlePatchesHeight][];
        Color[][] transformedLowerMiddlePatch = new Color[originalLowerPatchesHeight][];

        // TODO use Array.Copy(Array, int, Array, int, int) to copy ranges of elements (see Utils.Extensions.SetRange)

        // First loop: stretching the upper middle patch to the right by copying each row next to itself until the new width.
        // Height remains the same, width changes.
        for (int i = 0; i < transformedUpperMiddlePatch.Length; i++) // Outer loop goes over the rows of the new upper
                                                          //   middle patch (which counts the same as the original).
        {
            transformedUpperMiddlePatch[i] = new Color[newMiddlePatchesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedUpperMiddlePatch[i].Length; j++) // Inner loop goes over each individual element of the newly created array.
            {
                // Setting elements to the repeating pattern of the original rows.
                // By getting the modulo of 'j' and 'originalMiddlePatchesWidth'
                //   we can make sure that we repeat the original row until we reach the new width.
                transformedUpperMiddlePatch[i][j] = originalPatches[1][i][j % originalMiddlePatchesWidth];
            }
        }
        transformedPatches[1] = transformedUpperMiddlePatch;

        
        // Second loop: stretching the middle left patch downwards by repeating each row until the new height.
        // Height changes, width remains the same.
        for (int i = 0; i < transformedMiddleLeftPatch.Length; i++) // Outer loop goes over the rows of the new middle left patch.
        {
            transformedMiddleLeftPatch[i] = new Color[originalLeftPatchesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedMiddleLeftPatch[i].Length; j++) // Inner loop goes over each individual element of the
                                                                //   newly created array (which counts the same as the original).
            {
                // Setting elements from the original rows.
                // By getting the modulo of 'i' and 'originalMiddlePatchesHeight'
                //   we can make sure that we repeat each original row until we reach the new height.
                transformedMiddleLeftPatch[i][j] = originalPatches[3][i % originalMiddlePatchesHeight][j];
            }
        }
        transformedPatches[3] = transformedMiddleLeftPatch;

        // Third loop: stretching the central patch
        //   to the right by copying each row next to itself until the new width
        //   and downwards by repeating each row until the new height.
        // Height changes, width changes.
        for (int i = 0; i < transformedCentralPatch.Length; i++) // Outer loop goes over the rows of the new central patch.
        {
            transformedCentralPatch[i] = new Color[newMiddlePatchesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedCentralPatch[i].Length; j++) // Inner loop goes over each individual element of the newly created array.
            {
                // Setting elements to the repeating pattern of the original rows.
                // By getting the modulo of 'i' and 'originalMiddlePatchesHeight'
                //   we can make sure that we repeat each original row until we reach the new height.
                // By getting the modulo of 'j' and 'originalMiddlePatchesWidth'
                //   we can make sure that we repeat the original row until we reach the new width.
                transformedCentralPatch[i][j] = originalPatches[4][i % originalMiddlePatchesHeight][j % originalMiddlePatchesWidth];
            }
        }
        transformedPatches[4] = transformedCentralPatch;

        // Fourth loop: stretching the middle right patch downwards by repeating each row until the new height.
        // Height changes, width remains the same.
        for (int i = 0; i < transformedMiddleRightPatch.Length; i++) // Outer loop goes over the rows of the new middle right patch.
        {
            transformedMiddleRightPatch[i] = new Color[originalRightPatchesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedMiddleRightPatch[i].Length; j++) // Inner loop goes over each individual element of the
                                                                //   newly created array (which counts the same as the original).
            {
                // Setting elements from the original rows.
                // By getting the modulo of 'i' and 'originalMiddlePatchesHeight'
                //   we can make sure that we repeat each original row until we reach the new height.
                transformedMiddleRightPatch[i][j] = originalPatches[5][i % originalMiddlePatchesHeight][j];
            }
        }
        transformedPatches[5] = transformedMiddleRightPatch;

        // Fifth loop: stretching the lower middle patch to the right by copying each row next to itself until the new width.
        // Height remains the same, width changes.
        for (int i = 0; i < transformedLowerMiddlePatch.Length; i++) // Outer loop goes over the rows of the new lower
                                                          //   middle patch (which counts the same as the original).
        {
            transformedLowerMiddlePatch[i] = new Color[newMiddlePatchesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedLowerMiddlePatch[i].Length; j++) // Inner loop goes over each individual element of the newly created array.
            {
                // Setting elements to the repeating pattern of the original rows.
                // By getting the modulo of 'j' and 'originalMiddlePatchesWidth'
                //   we can make sure that we repeat the original row until we reach the new width.
                transformedLowerMiddlePatch[i][j] = originalPatches[7][i][j % originalMiddlePatchesWidth];
            }
        }
        transformedPatches[7] = transformedLowerMiddlePatch;

        return transformedPatches;
    }

    static Color[] MergePatchArrays(Color[][][] finalPatches, int width, int height)
    {
        int upperPatchesHeight = finalPatches[0].Length;
        int middlePatchesHeight = finalPatches[3].Length;
        int lowerPatchesHeight = finalPatches[6].Length;

        Color[] result = new Color[width * height];
        int resultElementCounter = 0;

        for (int i = 0; i < upperPatchesHeight; i++)
        {
            for (int j = 0; j < finalPatches[0][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[0][i][j];
            }
            for (int j = 0; j < finalPatches[1][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[1][i][j];
            }
            for (int j = 0; j < finalPatches[2][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[2][i][j];
            }
        }
        for (int i = 0; i < middlePatchesHeight; i++)
        {
            for (int j = 0; j < finalPatches[3][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[3][i][j];
            }
            for (int j = 0; j < finalPatches[4][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[4][i][j];
            }
            for (int j = 0; j < finalPatches[5][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[5][i][j];
            }
        }
        for (int i = 0; i < lowerPatchesHeight; i++)
        {
            for (int j = 0; j < finalPatches[6][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[6][i][j];
            }
            for (int j = 0; j < finalPatches[7][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[7][i][j];
            }
            for (int j = 0; j < finalPatches[8][i].Length; j++)
            {
                result[resultElementCounter++] = finalPatches[8][i][j];
            }
        }

        return result;
    }
}