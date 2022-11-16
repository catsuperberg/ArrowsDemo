using System;

namespace Utils
{
	public class FastRandom
	{
		// The +1 ensures NextDouble doesn't generate 1.0
		const double REAL_UNIT_INT = 1.0/((double)int.MaxValue+1.0);
		const double REAL_UNIT_UINT = 1.0/((double)uint.MaxValue+1.0);
		const uint Y=842502087, Z=3579807591, W=273326509;

		uint x, y, z, w;

		public FastRandom()
		{
			// Initialise using the system tick count.
			Reinitialise((int)Environment.TickCount);
		}

		public FastRandom(int seed)
		{
			Reinitialise(seed);
		}

		public void Reinitialise(int seed)
		{
			// The only stipulation stated for the xorshift RNG is that at least one of
			// the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
			// resetting of the x seed
			x = (uint)seed;
			y = Y;
			z = Z;
			w = W;
		}

		public int Next()
		{
			uint t=(x^(x<<11));
			x=y; y=z; z=w;
			w=(w^(w>>19))^(t^(t>>8));

			// Handle the special case where the value int.MaxValue is generated. This is outside of 
			// the range of permitted values, so we therefore call Next() to try again.
			uint rtn = w&0x7FFFFFFF;
			if(rtn==0x7FFFFFFF)
				return Next();
			return (int)rtn;			
		}

		public int Next(int upperBound)
		{
			if(upperBound<0)
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");

			uint t=(x^(x<<11));
			x=y; y=z; z=w;

			// The explicit int cast before the first multiplication gives better performance.
			// See comments in NextDouble.
			return (int)((REAL_UNIT_INT*(int)(0x7FFFFFFF&(w=(w^(w>>19))^(t^(t>>8)))))*upperBound);
		}

		public int Next(int lowerBound, int upperBound)
		{
			if(lowerBound>upperBound)
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");

			uint t=(x^(x<<11));
			x=y; y=z; z=w;

			// The explicit int cast before the first multiplication gives better performance.
			// See comments in NextDouble.
			int range = upperBound-lowerBound;
			if(range<0)
			{	// If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
				// We also must use all 32 bits of precision, instead of the normal 31, which again is slower.	
				return lowerBound+(int)((REAL_UNIT_UINT*(double)(w=(w^(w>>19))^(t^(t>>8))))*(double)((long)upperBound-(long)lowerBound));
			}
				
			// 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
			// a little more performance.
			return lowerBound+(int)((REAL_UNIT_INT*(double)(int)(0x7FFFFFFF&(w=(w^(w>>19))^(t^(t>>8)))))*(double)range);
		}

		public double NextDouble()
		{	
			uint t=(x^(x<<11));
			x=y; y=z; z=w;

			// Here we can gain a 2x speed improvement by generating a value that can be cast to 
			// an int instead of the more easily available uint. If we then explicitly cast to an 
			// int the compiler will then cast the int to a double to perform the multiplication, 
			// this final cast is a lot faster than casting from a uint to a double. The extra cast
			// to an int is very fast (the allocated bits remain the same) and so the overall effect 
			// of the extra cast is a significant performance improvement.
			//
			// Also note that the loss of one bit of precision is equivalent to what occurs within 
			// System.Random.
			return (REAL_UNIT_INT*(int)(0x7FFFFFFF&(w=(w^(w>>19))^(t^(t>>8)))));			
		}

		public void NextBytes(byte[] buffer)
		{
			// Fill up the bulk of the buffer in chunks of 4 bytes at a time.
			uint x=this.x, y=this.y, z=this.z, w=this.w;
			int i=0;
			uint t;
			for(int bound=buffer.Length-3; i<bound;)
			{	
				// Generate 4 bytes. 
				// Increased performance is achieved by generating 4 random bytes per loop.
				// Also note that no mask needs to be applied to zero out the higher order bytes before
				// casting because the cast ignores thos bytes. Thanks to Stefan Trosch�tz for pointing this out.
				t=(x^(x<<11));
				x=y; y=z; z=w;
				w=(w^(w>>19))^(t^(t>>8));

				buffer[i++] = (byte)w;
				buffer[i++] = (byte)(w>>8);
				buffer[i++] = (byte)(w>>16);
				buffer[i++] = (byte)(w>>24);
			}

			// Fill up any remaining bytes in the buffer.
			if(i<buffer.Length)
			{
				// Generate 4 bytes.
				t=(x^(x<<11));
				x=y; y=z; z=w;
				w=(w^(w>>19))^(t^(t>>8));

				buffer[i++] = (byte)w;
				if(i<buffer.Length)
				{
					buffer[i++]=(byte)(w>>8);
					if(i<buffer.Length)
					{	
						buffer[i++] = (byte)(w>>16);
						if(i<buffer.Length)
						{	
							buffer[i] = (byte)(w>>24);
						}
					}
				}
			}
			this.x=x; this.y=y; this.z=z; this.w=w;
		}

		public uint NextUInt()
		{
			uint t=(x^(x<<11));
			x=y; y=z; z=w;
			return (w=(w^(w>>19))^(t^(t>>8)));
		}

		public int NextInt()
		{
			uint t=(x^(x<<11));
			x=y; y=z; z=w;
			return (int)(0x7FFFFFFF&(w=(w^(w>>19))^(t^(t>>8))));
		}


		// Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
		// with bitBufferIdx.
		uint bitBuffer;
		uint bitMask=1;

		public bool NextBool()
		{
			if(bitMask==1)
			{	
				// Generate 32 more bits.
				uint t=(x^(x<<11));
				x=y; y=z; z=w;
				bitBuffer=w=(w^(w>>19))^(t^(t>>8));

				// Reset the bitMask that tells us which bit to read next.
				bitMask = 0x80000000;
				return (bitBuffer & bitMask)==0;
			}

			return (bitBuffer & (bitMask>>=1))==0;
		}
	}
}
