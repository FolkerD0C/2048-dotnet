using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameClient.App.Utils;

internal static class NineSliceUtils
{
    internal enum NineSliceMethod
    {
        Simple,
        Repeating,
        Stretching
    }

    internal static Texture2D EnlargeSlices(
        GraphicsDevice gd, Texture2D texture, NineSliceMethod method,
        int frameCount, int destinationWidth, int destinationHeight,
        int horizontalTertile1 = -1, int horizontalTertile2 = -1,
        int verticalTertile1 = -1, int verticalTertile2 = -1
    )
    {
        var frames = GetFrames(texture, frameCount);
        Color[][] slicedFrames = new Color[frameCount][];
        int sourceWidth = texture.Width;
        int sourceHeight = texture.Height / frameCount;

        for (int i = 0; i < frameCount; i++)
        {
            switch (method) // TODO implement the Stretching method
            {
                case NineSliceMethod.Simple:
                {
                    slicedFrames[1] = SimpleNineSlice(
                        frames[i], sourceWidth, sourceHeight, destinationWidth, destinationHeight
                    );
                    break;
                }
                case NineSliceMethod.Repeating:
                {
                    slicedFrames[i] = RepeatingNineSlice(
                        frames[i], sourceWidth, sourceHeight, destinationWidth, destinationHeight,
                        horizontalTertile1, horizontalTertile2, verticalTertile1, verticalTertile2
                    );
                    break;
                }
                default:
                    break;
            }
        }
        return MergeFrames(gd, slicedFrames, destinationWidth, destinationHeight);
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
            rawResult.SetRange(frames[i].Length * i, frames[i]);
        }
        result.SetData(rawResult);

        return result;
    }

    static Color[] SimpleNineSlice(
        Color[] rawData, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight
    )
    {
        if (sourceWidth % 2 != 1 || sourceHeight % 2 != 1)
        {
            throw new InvalidOperationException("Central proportions must be 1 pixel wide/tall,"
                + " the others must match in width/heigth with the opposite side.");
        }
        int horizontalTertile1 = sourceWidth / 2;
        int horizontalTertile2 = sourceWidth / 2 + 1;
        int verticalTertile1 = sourceHeight / 2;
        int verticalTertile2 = sourceHeight / 2 + 1;

        Color[][][] corners = new Color[4][][];
        corners[0] = new Color[verticalTertile1][];
        corners[1] = new Color[verticalTertile1][];
        corners[2] = new Color[verticalTertile1][];
        corners[3] = new Color[verticalTertile1][];

        for (int i = 0; i < verticalTertile1; i++)
        {
            corners[0][i] = rawData[(sourceWidth * i)..(sourceWidth * i + horizontalTertile1)];
            corners[1][i] = rawData[(sourceWidth * i + horizontalTertile2)..(sourceWidth * (i + 1))];
        }
        for (int i = verticalTertile2; i < sourceHeight; i++)
        {
            corners[2][i - verticalTertile2] = rawData[(sourceWidth * i)..(sourceWidth * i + horizontalTertile1)];
            corners[3][i - verticalTertile2] = rawData[(sourceWidth * i + horizontalTertile2)..(sourceWidth * (i + 1))];
        }

        Color[][] edges = new Color[4][];
        edges[0] = new Color[verticalTertile1];
        for (int i = 0; i < verticalTertile1; i++)
        {
            edges[0][i] = rawData[sourceWidth * i + horizontalTertile1];
        }
        edges[1] = rawData[(sourceWidth * verticalTertile1)..(sourceWidth * verticalTertile1 + horizontalTertile1)];
        edges[2] = rawData[(sourceWidth * verticalTertile1 + horizontalTertile2)..(sourceWidth * verticalTertile2)];
        edges[3] = new Color[verticalTertile1];
        for (int i = 0; i < verticalTertile1; i++)
        {
            edges[3][i] = rawData[sourceWidth * verticalTertile2 + sourceWidth * i + horizontalTertile1];
        }

        Color centralPixel = rawData[sourceWidth * verticalTertile1 + horizontalTertile1];

        var simpleSliceArrays = CreateSimpleSliceArrays(
            corners, edges, centralPixel, destinationWidth - sourceWidth + 1, destinationHeight - sourceHeight + 1
        );

        return MergeSliceArrays(simpleSliceArrays, destinationWidth, destinationHeight);
    }

    static Color[] RepeatingNineSlice(
        Color[] rawData, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight,
        int horizontalTertile1, int horizontalTertile2, int verticalTertile1, int verticalTertile2
    )
    {
        Color[][][] proportions = InitiateSliceArrays(verticalTertile1, verticalTertile2, sourceHeight);

        for (int i = 0; i < verticalTertile1; i++)
        {
            proportions[0][i] = rawData[(sourceWidth * i)..(sourceWidth * i + horizontalTertile1)];
            proportions[1][i] = rawData[(sourceWidth * i + horizontalTertile1)..(sourceWidth * i + horizontalTertile2)];
            proportions[2][i] = rawData[(sourceWidth * i + horizontalTertile2)..(sourceWidth * (i + 1))];
        }

        for (int i = verticalTertile1; i < verticalTertile2; i++)
        {
            proportions[3][i - verticalTertile1] = rawData[(sourceWidth * i)..(sourceWidth * i + horizontalTertile1)];
            proportions[4][i - verticalTertile1] = rawData[(sourceWidth * i + horizontalTertile1)..(sourceWidth * i + horizontalTertile2)];
            proportions[5][i - verticalTertile1] = rawData[(sourceWidth * i + horizontalTertile2)..(sourceWidth * (i + 1))];
        }

        for (int i = verticalTertile2; i < sourceHeight; i++)
        {
            proportions[6][i - verticalTertile2] = rawData[(sourceWidth * i)..(sourceWidth * i + horizontalTertile1)];
            proportions[7][i - verticalTertile2] = rawData[(sourceWidth * i + horizontalTertile1)..(sourceWidth * i + horizontalTertile2)];
            proportions[8][i - verticalTertile2] = rawData[(sourceWidth * i + horizontalTertile2)..(sourceWidth * (i + 1))];
        }

        var repeatingSliceArrays = CreateRepeatingSliceArrays(
            proportions, destinationWidth - horizontalTertile1 - (sourceWidth - horizontalTertile2),
            destinationHeight - verticalTertile1 - (sourceHeight - verticalTertile2)
        );

        return MergeSliceArrays(repeatingSliceArrays, destinationWidth, destinationHeight);
    }

    static Color[][][] InitiateSliceArrays(int verticalTertile1, int verticalTertile2, int originalHeight)
    {
        Color[][][] slices = new Color[9][][];
        slices[0] = new Color[verticalTertile1][];
        slices[1] = new Color[verticalTertile1][];
        slices[2] = new Color[verticalTertile1][];
        slices[3] = new Color[verticalTertile2 - verticalTertile1][];
        slices[4] = new Color[verticalTertile2 - verticalTertile1][];
        slices[5] = new Color[verticalTertile2 - verticalTertile1][];
        slices[6] = new Color[originalHeight - verticalTertile2][];
        slices[7] = new Color[originalHeight - verticalTertile2][];
        slices[8] = new Color[originalHeight - verticalTertile2][];
        return slices;
    }

    static Color[][][] CreateSimpleSliceArrays(
        Color[][][] corners, Color[][] edges, Color centralPixel, int newMiddleSlicesWidth, int newMiddleSlicesHeight
    )
    {
        Color[][][] transformedSlices = new Color[9][][];
        transformedSlices[0] = corners[0];
        transformedSlices[2] = corners[1];
        transformedSlices[6] = corners[2];
        transformedSlices[8] = corners[3];

        int upperLowerSlicesHeight = edges[0].Length;
        int leftRightSlicesWidth = edges[1].Length;
        
        Color[][] transformedUpperMiddleSlice = new Color[upperLowerSlicesHeight][];
        Color[][] transformedMiddleLeftSlice = new Color[newMiddleSlicesHeight][];
        Color[][] transformedCentralSlice = new Color[newMiddleSlicesHeight][];
        Color[][] transformedMiddleRightSlice = new Color[newMiddleSlicesHeight][];
        Color[][] transformedLowerMiddleSlice = new Color[upperLowerSlicesHeight][];

        for (int i = 0; i < transformedUpperMiddleSlice.Length; i++)
        {
            transformedUpperMiddleSlice[i] = new Color[newMiddleSlicesWidth];
            Array.Fill(transformedUpperMiddleSlice[i], edges[0][i]);
        }
        transformedSlices[1] = transformedUpperMiddleSlice;

        for (int i = 0; i < transformedMiddleLeftSlice.Length; i++)
        {
            transformedMiddleLeftSlice[i] = new Color[leftRightSlicesWidth];
            Array.Copy(edges[1], transformedMiddleLeftSlice[i], edges[1].Length);
        }
        transformedSlices[3] = transformedMiddleLeftSlice;

        for (int i = 0; i < transformedCentralSlice.Length; i++)
        {
            transformedCentralSlice[i] = new Color[newMiddleSlicesWidth];
            Array.Fill(transformedCentralSlice[i], centralPixel);
        }
        transformedSlices[4] = transformedCentralSlice;

        for (int i = 0; i < transformedMiddleRightSlice.Length; i++)
        {
            transformedMiddleRightSlice[i] = new Color[leftRightSlicesWidth];
            Array.Copy(edges[2], transformedMiddleRightSlice[i], edges[2].Length);
        }
        transformedSlices[5] = transformedMiddleRightSlice;

        for (int i = 0; i < transformedLowerMiddleSlice.Length; i++)
        {
            transformedLowerMiddleSlice[i] = new Color[newMiddleSlicesWidth];
            Array.Fill(transformedLowerMiddleSlice[i], edges[3][i]);
        }
        transformedSlices[7] = transformedLowerMiddleSlice;

        return transformedSlices;
    }

    static Color[][][] CreateRepeatingSliceArrays(Color[][][] originalSlices, int newMiddleSlicesWidth, int newMiddleSlicesHeight)
    {
        Color[][][] transformedSlices = new Color[9][][];
        transformedSlices[0] = originalSlices[0];
        transformedSlices[2] = originalSlices[2];
        transformedSlices[6] = originalSlices[6];
        transformedSlices[8] = originalSlices[8];

        int originalUpperSlicesHeight = originalSlices[0].Length;
        int originalMiddleSlicesHeight = originalSlices[3].Length;
        int originalLowerSlicesHeight = originalSlices[6].Length;
        int originalLeftSlicesWidth = originalSlices[0][0].Length;
        int originalMiddleSlicesWidth = originalSlices[1][0].Length;
        int originalRightSlicesWidth = originalSlices[2][0].Length;

        Color[][] transformedUpperMiddleSlice = new Color[originalUpperSlicesHeight][];
        Color[][] transformedMiddleLeftSlice = new Color[newMiddleSlicesHeight][];
        Color[][] transformedCentralSlice = new Color[newMiddleSlicesHeight][];
        Color[][] transformedMiddleRightSlice = new Color[newMiddleSlicesHeight][];
        Color[][] transformedLowerMiddleSlice = new Color[originalLowerSlicesHeight][];

        // First loop: stretching the upper middle slice to the right by copying each row next to itself until the new width.
        // Height remains the same, width changes.
        for (int i = 0; i < transformedUpperMiddleSlice.Length; i++) // Outer loop goes over the rows of the new upper
                                                          //   middle slice (which counts the same as the original).
        {
            transformedUpperMiddleSlice[i] = new Color[newMiddleSlicesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedUpperMiddleSlice[i].Length; j++) // Inner loop goes over each individual element of the newly created array.
            {
                // Setting elements to the repeating pattern of the original rows.
                // By getting the modulo of 'j' and 'originalMiddleSlicesWidth'
                //   we can make sure that we repeat the original row until we reach the new width.
                transformedUpperMiddleSlice[i][j] = originalSlices[1][i][j % originalMiddleSlicesWidth];
            }
        }
        transformedSlices[1] = transformedUpperMiddleSlice;

        
        // Second loop: stretching the middle left slice downwards by repeating each row until the new height.
        // Height changes, width remains the same.
        for (int i = 0; i < transformedMiddleLeftSlice.Length; i++) // Outer loop goes over the rows of the new middle left slice.
        {
            transformedMiddleLeftSlice[i] = new Color[originalLeftSlicesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedMiddleLeftSlice[i].Length; j++) // Inner loop goes over each individual element of the
                                                                //   newly created array (which counts the same as the original).
            {
                // Setting elements from the original rows.
                // By getting the modulo of 'i' and 'originalMiddleSlicesHeight'
                //   we can make sure that we repeat each original row until we reach the new height.
                transformedMiddleLeftSlice[i][j] = originalSlices[3][i % originalMiddleSlicesHeight][j];
            }
        }
        transformedSlices[3] = transformedMiddleLeftSlice;

        // Third loop: stretching the central slice
        //   to the right by copying each row next to itself until the new width
        //   and downwards by repeating each row until the new height.
        // Height changes, width changes.
        for (int i = 0; i < transformedCentralSlice.Length; i++) // Outer loop goes over the rows of the new central slice.
        {
            transformedCentralSlice[i] = new Color[newMiddleSlicesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedCentralSlice[i].Length; j++) // Inner loop goes over each individual element of the newly created array.
            {
                // Setting elements to the repeating pattern of the original rows.
                // By getting the modulo of 'i' and 'originalMiddleSlicesHeight'
                //   we can make sure that we repeat each original row until we reach the new height.
                // By getting the modulo of 'j' and 'originalMiddleSlicesWidth'
                //   we can make sure that we repeat the original row until we reach the new width.
                transformedCentralSlice[i][j] = originalSlices[4][i % originalMiddleSlicesHeight][j % originalMiddleSlicesWidth];
            }
        }
        transformedSlices[4] = transformedCentralSlice;

        // Fourth loop: stretching the middle right slice downwards by repeating each row until the new height.
        // Height changes, width remains the same.
        for (int i = 0; i < transformedMiddleRightSlice.Length; i++) // Outer loop goes over the rows of the new middle right slice.
        {
            transformedMiddleRightSlice[i] = new Color[originalRightSlicesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedMiddleRightSlice[i].Length; j++) // Inner loop goes over each individual element of the
                                                                //   newly created array (which counts the same as the original).
            {
                // Setting elements from the original rows.
                // By getting the modulo of 'i' and 'originalMiddleSlicesHeight'
                //   we can make sure that we repeat each original row until we reach the new height.
                transformedMiddleRightSlice[i][j] = originalSlices[5][i % originalMiddleSlicesHeight][j];
            }
        }
        transformedSlices[5] = transformedMiddleRightSlice;

        // Fifth loop: stretching the lower middle slice to the right by copying each row next to itself until the new width.
        // Height remains the same, width changes.
        for (int i = 0; i < transformedLowerMiddleSlice.Length; i++) // Outer loop goes over the rows of the new lower
                                                          //   middle slice (which counts the same as the original).
        {
            transformedLowerMiddleSlice[i] = new Color[newMiddleSlicesWidth]; // Creating new array at each row.
            for (int j = 0; j < transformedLowerMiddleSlice[i].Length; j++) // Inner loop goes over each individual element of the newly created array.
            {
                // Setting elements to the repeating pattern of the original rows.
                // By getting the modulo of 'j' and 'originalMiddleSlicesWidth'
                //   we can make sure that we repeat the original row until we reach the new width.
                transformedLowerMiddleSlice[i][j] = originalSlices[7][i][j % originalMiddleSlicesWidth];
            }
        }
        transformedSlices[7] = transformedLowerMiddleSlice;

        return transformedSlices;
    }

    static Color[] MergeSliceArrays(Color[][][] finalSlices, int width, int height)
    {
        int upperSlicesHeight = finalSlices[0].Length;
        int middleSlicesHeight = finalSlices[3].Length;
        int lowerSlicesHeight = finalSlices[6].Length;

        Color[] result = new Color[width * height];
        int resultElementCounter = 0;

        for (int i = 0; i < upperSlicesHeight; i++)
        {
            result.SetRange(resultElementCounter, finalSlices[0][i]);
            resultElementCounter += finalSlices[0][i].Length;
            result.SetRange(resultElementCounter, finalSlices[1][i]);
            resultElementCounter += finalSlices[1][i].Length;
            result.SetRange(resultElementCounter, finalSlices[2][i]);
            resultElementCounter += finalSlices[2][i].Length;
        }
        for (int i = 0; i < middleSlicesHeight; i++)
        {
            result.SetRange(resultElementCounter, finalSlices[3][i]);
            resultElementCounter += finalSlices[3][i].Length;
            result.SetRange(resultElementCounter, finalSlices[4][i]);
            resultElementCounter += finalSlices[4][i].Length;
            result.SetRange(resultElementCounter, finalSlices[5][i]);
            resultElementCounter += finalSlices[5][i].Length;
        }
        for (int i = 0; i < lowerSlicesHeight; i++)
        {
            result.SetRange(resultElementCounter, finalSlices[6][i]);
            resultElementCounter += finalSlices[6][i].Length;
            result.SetRange(resultElementCounter, finalSlices[7][i]);
            resultElementCounter += finalSlices[7][i].Length;
            result.SetRange(resultElementCounter, finalSlices[8][i]);
            resultElementCounter += finalSlices[8][i].Length;
        }

        return result;
    }
}