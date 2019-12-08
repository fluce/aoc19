using System;
using Xunit;
using Day8;

namespace Day8.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("123456",3,2,0)]
        [InlineData("789012",3,2,1)]
        public void TestLayer(string data, int width, int height, int expected0Count)
        {
            var layer=new Layer(data.ToCharArray(),width,height);
            Assert.Equal(expected0Count, layer.Count('0'));
        }

        [Theory]
        [InlineData("123456789012",3,2,2)]
        public void TestImage(string data, int width, int height, int expectedLayerCount)
        {
            var image=new Image(data.ToCharArray(),width,height);
            Assert.Equal(expectedLayerCount, image.Layers.Count);
        }

        [Theory]
        [InlineData("123456789012",3,2,0)]
        [InlineData("123006789012",3,2,1)]
        public void TestFindBestLayer(string data, int width, int height, int expectedBestLayer)
        {
            var image=new Image(data.ToCharArray(),width,height);
            var bestLayer=image.FindBestLayer();
            Assert.Equal(expectedBestLayer,image.Layers.IndexOf(bestLayer));
        }

        [Theory]
        [InlineData('0','0','0')]
        [InlineData('0','1','1')]
        [InlineData('0','2','0')]
        [InlineData('1','0','0')]
        [InlineData('1','1','1')]
        [InlineData('1','2','1')]
        [InlineData('2','0','0')]
        [InlineData('2','1','1')]
        [InlineData('2','2','2')]
        public void TestCombineOne(char back, char front, char expected)
        {
            Assert.Equal(expected,Image.Combine(back,front));
        }

        [Theory]
        [InlineData("0222112222120000",2,2,"0110")]
        public void TestCombineLayer(string data, int width, int height, string expectedLayer)
        {
            var image=new Image(data.ToCharArray(),width,height);
            var layer=image.Combine();
            var expected=new Layer(expectedLayer.ToCharArray(),width, height);
            Assert.Equal(expected.Data,layer.Data);
        }
    }
}
