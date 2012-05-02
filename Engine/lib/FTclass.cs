
#pragma warning disable
#region Header
// --------------------------------------------------------------------------
//    Icarus Scene Engine
//    Copyright (C) 2005-2007  Euan D. MacInnes. All Rights Reserved.

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// --------------------------------------------------------------------------
//
//     UNIT               : FTclass.cs
//     SUMMARY            : Tao.FreeType OpenGL wrapper for uploading FreeType fonts to
//                        : OpenGL
//                        : 
//                        : 
//
//     PRINCIPLE AUTHOR   : Euan D. MacInnes
//     
#endregion Header
#region Revisions
// --------------------------------------------------------------------------
//     REVISIONS/NOTES
//        dd-mm-yyyy    By          Revision Summary
//
// --------------------------------------------------------------------------
#endregion Revisions 


using System;
using System.Collections.Generic;
using System.Text;
using Tao.FreeType;
using Tao.OpenGl;
using System.Runtime.InteropServices;
using System.IO;

namespace ISE
{
    // for reporting errors
    public delegate void OnWriteEventType(string EventDetails);

    public enum FontRender { Outline, Filled, Texture };

	/// <summary>
	/// Glyph offset information for advanced rendering and/or conversions.
	/// </summary>
	public struct FTGlyphOffset
	{
		/// <summary>
		/// Width of the Glyph, in pixels.
		/// </summary>
		public int width;
		/// <summary>
		/// height of the Glyph, in pixels. Represents the number of scanlines
		/// </summary>
		public int height;
		/// <summary>
		/// For Bitmap-generated fonts, this is the top-bearing expressed in integer pixels.
		/// This is the distance from the baseline to the topmost Glyph scanline, upwards Y being positive.
		/// </summary>
		public int top;
		/// <summary>
		/// For Bitmap-generated fonts, this is the left-bearing expressed in integer pixels
		/// </summary>
		public int left;
		/// <summary>
		/// This is the transformed advance width for the glyph.
		/// </summary>
		public FT_Vector advance;
		/// <summary>
		/// The difference between hinted and unhinted left side bearing while autohinting is active. 0 otherwise.
		/// </summary>
		public long lsb_delta;
		/// <summary>
		/// The difference between hinted and unhinted right side bearing while autohinting is active. 0 otherwise.
		/// </summary>
		public long rsb_delta;
		/// <summary>
		/// The advance width of the unhinted glyph. Its value is expressed in 16.16 fractional pixels, unless FT_LOAD_LINEAR_DESIGN is set when loading the glyph. This field can be important to perform correct WYSIWYG layout. Only relevant for outline glyphs.
		/// </summary>
		public long linearHoriAdvance;
		/// <summary>
		/// The advance height of the unhinted glyph. Its value is expressed in 16.16 fractional pixels, unless FT_LOAD_LINEAR_DESIGN is set when loading the glyph. This field can be important to perform correct WYSIWYG layout. Only relevant for outline glyphs.
		/// </summary>
		public long linearVertAdvance;
	}
	
	/// <summary>
	/// For internal use, to represent the type of conversion to apply to the font
	/// </summary>
    public enum FTFontType 
    { 
        /// <summary>
        /// Font has not been initialised yet
        /// </summary>
        FT_NotInitialised, 
        /// <summary>
        /// Font was converted to a series of Textures
        /// </summary>
        FT_Texture, 
        /// <summary>
        /// Font was converted to a big texture map, representing a collection of glyphs
        /// </summary>
        FT_TextureMap, 
        /// <summary>
        /// Font was converted to outlines and stored as display lists
        /// </summary>
        FT_Outline, 
        /// <summary>
        /// Font was convered to Outliens and stored as Vertex Buffer Objects
        /// </summary>
        FT_OutlineVBO 
    }
	
	/// <summary>
    /// Alignment of output text
	/// </summary> 
	public enum FTFontAlign
    {
        /// <summary>
        /// Left-align the text when it is drawn
        /// </summary>
        FT_ALIGN_LEFT, 
        /// <summary>
        /// Center-align the text when it is drawn
        /// </summary>
        FT_ALIGN_CENTERED, 
        /// <summary>
        /// Right-align the text when it is drawn
        /// </summary>
        FT_ALIGN_RIGHT 
    }

    public static class FreeType
    {
        /// <summary>
        /// This event reports on the status of FreeType.
        /// This is useful to assign to this event to record down
        /// FreeType output to a debug log file, for example.
        /// </summary>
        public static OnWriteEventType OnWriteEvent;

        // Global FreeType library pointer
        public static System.IntPtr libptr = IntPtr.Zero;

        public static FTFont DefaultFont;
    }

    /// <summary>
    /// Font class wraper for displaying FreeType fonts in OpenGL.
    /// </summary>
    public class FTFont
    {


        //Public members        
        private int list_base;
        private int font_size = 48;
        private static int max_chars = 70000;

        private int font_max_chars = 0;


        // Whether the font was loaded Ok or not
        private bool fontloaded = false;
        private int[] textures;
        private int[] extent_x;
		private FTGlyphOffset[] offsets;

        private System.IntPtr faceptr;
        private FT_FaceRec face;
		
		// debug variable used to list the state of all characters rendered
        private string sChars = "";  

        private static void Report(string ErrorText)
        {
            if (FreeType.OnWriteEvent != null)
                FreeType.OnWriteEvent(ErrorText);
            else
                Console.WriteLine(ErrorText);
        }
		
		/// <summary>
		/// Initialise the FreeType library
		/// </summary>
		/// <returns></returns>
		public static int ftInit()
		{
            // We begin by creating a library pointer            
            if (FreeType.libptr == IntPtr.Zero)
            {
                int ret = FT.FT_Init_FreeType(out FreeType.libptr);

                if (ret != 0)
                {
                    Report("Failed to start FreeType");
                }
                else
                {
                    Report("FreeType Loaded.");
                    Report("FreeType Version " + ftVersionString());
                }

                return ret;
            }
			
			return 0;
		}
		
		/// <summary>
		/// Font alignment public parameter
		/// </summary>		
		public FTFontAlign FT_ALIGN = FTFontAlign.FT_ALIGN_LEFT; 

        /// <summary>
        /// Initialise the Font. Will Initialise the freetype library if not already done so
        /// </summary>
        /// <param name="resourcefilename">Path to the external font file</param>
        /// <param name="Success">Returns 0 if successful</param>
        public FTFont(string resourcefilename, out int Success)
        {
            Report("Creating Font " + resourcefilename);
			Success = ftInit();          


            if (FreeType.libptr == IntPtr.Zero) { Report("Couldn't start FreeType"); Success = -1; return; }

			string fontfile = resourcefilename;
            
            //Once we have the library we create and load the font face                       
            int retb = FT.FT_New_Face(FreeType.libptr, fontfile, 0, out faceptr);
            if (retb != 0)
            {
                if (!File.Exists(fontfile))
                    Report(fontfile + " not found.");
                else
                    Report(fontfile + " found.");

                Report("Failed to load font " + fontfile + " (error code " + retb.ToString() + ")");
                fontloaded = true;
                Success = retb;
                return;
            }
            fontloaded = true;
                 
            Success = 0;
        }
		
        /// <summary>
        /// Return the version information for FreeType.
        /// </summary>
        /// <param name="Major">Major Version</param>
        /// <param name="Minor">Minor Version</param>
        /// <param name="Patch">Patch Number</param>
		public static void ftVersion(ref int Major, ref int Minor, ref int Patch)
		{
			ftInit();
            FT.FT_Library_Version(FreeType.libptr, ref Major, ref Minor, ref Patch);
		}
		
        /// <summary>
        /// Return the entire version information for FreeType as a String.
        /// </summary>
        /// <returns></returns>
		public static string ftVersionString()
		{
			int major = 0;
			int minor = 0;
			int patch = 0;
			ftVersion(ref major, ref minor, ref patch);
			return major.ToString() + "." + minor.ToString() + "." + patch.ToString();
		}

        public bool kerning = true;

        public bool Kerning { get { return kerning; } set { kerning = value; } }

        /// <summary>
        /// Render the font to a series of OpenGL textures (one per letter)
        /// </summary>
        /// <param name="fontsize">size of the font</param>
        /// <param name="DPI">dots-per-inch setting</param>
        public void ftRenderToTexture(int fontsize, uint DPI)
        {
            if (!fontloaded) return; 
            font_size = fontsize;

            if (faceptr == IntPtr.Zero)
            {
                Report("ERROR: No Face Pointer. Font was not created properly");
                return;
            }

            face = (FT_FaceRec)Marshal.PtrToStructure(faceptr, typeof(FT_FaceRec));
            Report("Num Faces:" + face.num_faces.ToString());
            Report("Num Glyphs:" + face.num_glyphs.ToString());
            Report("Num Char Maps:" + face.num_charmaps.ToString());
            Report("Font Family:" + face.family_name);
            Report("Style Name:" + face.style_name);
            Report("Generic:" + face.generic);
            Report("Bbox:" + face.bbox);
            Report("Glyph:" + face.glyph);
            //kerning = FT.FT_HAS_KERNING(faceptr);

            font_max_chars = (int)face.num_glyphs; 

         //   IConsole.Write("Num Glyphs:", );

            //Freetype measures the font size in 1/64th of pixels for accuracy 
            //so we need to request characters in size*64
            FT.FT_Set_Char_Size(faceptr, font_size << 6, font_size << 6, DPI, DPI);

            //Provide a reasonably accurate estimate for expected pixel sizes
            //when we later on create the bitmaps for the font
            FT.FT_Set_Pixel_Sizes(faceptr, (uint)font_size, (uint)font_size);
            

            // Once we have the face loaded and sized we generate opengl textures 
            // from the glyphs  for each printable character
            Report("Compiling Font Characters 0.." + max_chars.ToString());
            textures = new int[max_chars];
            extent_x = new int[max_chars];
            offsets = new FTGlyphOffset[max_chars];
            list_base = Gl.glGenLists(max_chars);
            //Console.WriteLine("Font Compiled:" + sChars);            
        }

        internal int next_po2(int a)
        {
            int rval = 1;
            while (rval < a) rval <<= 1;
            return rval;
        }

        private void CheckGlyph(int c)
        {
            if (textures[c] == 0)
            {
                Gl.glGenTextures(1, out textures[c]);
                CompileCharacter(face, c);
            }
        }

        private void RenderAsTexture(FT_GlyphSlotRec glyphrec, int c)
        {
            int ret = FT.FT_Render_Glyph(ref glyphrec, FT_Render_Mode.FT_RENDER_MODE_NORMAL);

            FTGlyphOffset offset = new FTGlyphOffset();

            if (ret != 0)
            {
                Report("Render failed for character " + c.ToString());
            }

            string sError = "";

            int size = (glyphrec.bitmap.width * glyphrec.bitmap.rows);
            if (size <= 0)
            {
                //Console.Write("Blank Character: " + c.ToString());
                //space is a special `blank` character
                extent_x[c] = 0;
                if (c == 32)
                {
                    Gl.glNewList((list_base + c), Gl.GL_COMPILE);
                    Gl.glTranslatef(font_size >> 1, 0, 0);
                    extent_x[c] = font_size >> 1;
                    Gl.glEndList();
                    offset.left = 0;
                    offset.top = 0;
                    offset.height = 0;
                    offset.width = extent_x[c];
                    offsets[c] = offset;
                }
                return;

            }

            byte[] bmp = new byte[size];
            Marshal.Copy(glyphrec.bitmap.buffer, bmp, 0, bmp.Length);

            //Next we expand the bitmap into an opengl texture 	    	
            int width = next_po2(glyphrec.bitmap.width);
            int height = next_po2(glyphrec.bitmap.rows);

            byte[, ,] expanded = new byte[height + 2, width + 2, 2];

            for (int j = 0; j < height + 2; j++)
            {
                for (int i = 0; i < width + 2; i++)
                {
                    //Luminance
                    expanded[j, i, 0] = (byte)255;
                    expanded[j, i, 1] = 0;
                }
            }

            for (int j = 0; j < height; j++)
            {               
                for (int i = 0; i < width; i++)
                {
                    //expanded[4 * (i + j * width) + 1] = (byte)255;
                    //expanded[4 * (i + j * width) + 2] = (byte)255;

                    // Alpha
                    expanded[j + 1, i + 1,1] =
                        (i >= glyphrec.bitmap.width || j >= glyphrec.bitmap.rows) ?
                        (byte)0 : (byte)(bmp[i + glyphrec.bitmap.width * j]);
                }
            }            

            //Set up some texture parameters for opengl
            Gl.glBindTexture(Engine.Renderer.GlTextureMode, textures[c]);
            Gl.glTexParameteri(Engine.Renderer.GlTextureMode, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Engine.Renderer.GlTextureMode, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Engine.Renderer.GlTextureMode, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Engine.Renderer.GlTextureMode, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);

            //Create the texture
            Gl.glTexImage2D(Engine.Renderer.GlTextureMode, 0, Gl.GL_RGBA, width +2, height+2,
                0, Gl.GL_LUMINANCE_ALPHA, Gl.GL_UNSIGNED_BYTE, expanded);
            expanded = null;
            bmp = null;

            //Create a display list and bind a texture to it
            Gl.glNewList((list_base + c), Gl.GL_COMPILE);
            Gl.glBindTexture(Engine.Renderer.GlTextureMode, textures[c]);

            //Account for freetype spacing rules
            Gl.glTranslatef(glyphrec.bitmap_left, 0, 0);
            Gl.glPushMatrix();
            Gl.glTranslatef(0, glyphrec.bitmap_top - glyphrec.bitmap.rows, 0);
            double x = Engine.Renderer.TextureCoordinate(glyphrec.bitmap.width, width);
            double y = Engine.Renderer.TextureCoordinate(glyphrec.bitmap.rows, height);

            offset.left = glyphrec.bitmap_left;
            offset.top = glyphrec.bitmap_top;
            offset.height = glyphrec.bitmap.rows;
            offset.width = glyphrec.bitmap.width;
            offset.advance = glyphrec.advance;
            offset.lsb_delta = glyphrec.lsb_delta;
            offset.rsb_delta = glyphrec.rsb_delta;
            offset.linearHoriAdvance = glyphrec.linearHoriAdvance;
            offset.linearVertAdvance = glyphrec.linearVertAdvance;
            offsets[c] = offset;

            //Draw the quad
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0, 0); Gl.glVertex2f(-1, glyphrec.bitmap.rows+1);
            Gl.glTexCoord2d(0, y); Gl.glVertex2f(-1, -1);
            Gl.glTexCoord2d(x, y); Gl.glVertex2f(glyphrec.bitmap.width+1, -1);
            Gl.glTexCoord2d(x, 0); Gl.glVertex2f(glyphrec.bitmap.width+1, glyphrec.bitmap.rows+1);
            Gl.glEnd();
            Gl.glPopMatrix();
            
            //Advance for the next character			
            Gl.glTranslatef(glyphrec.advance.x >> 6, 0, 0);
            extent_x[c] = glyphrec.bitmap_left + (glyphrec.advance.x >> 6);
            Gl.glEndList();
            sChars += "f:" + c.ToString() + "[w:" + glyphrec.bitmap.width.ToString() + "][h:" + glyphrec.bitmap.rows.ToString() + "]" + sError;
        }

        private bool contour_open_ = false;
        private Glu.GLUtesselator tess_obj_;

        /*private void RenderAsOutline(FT_GlyphSlotRec glyphrec, int c)
        {
            Gl.glNewList((list_base + c), Gl.GL_COMPILE);

            tess_obj_ = Glu.gluNewTess();*/

            //Glu.gluTessCallback(tess_obj_, Glu.GLU_TESS_VERTEX, vertexCallback);
            //Glu.gluTessCallback(tess_obj_, Glu.GLU_TESS_BEGIN, beginCallback);
            //Glu.gluTessCallback(tess_obj_, Glu.GLU_TESS_END, endCallback);
            //Glu.gluTessCallback(tess_obj_, Glu.GLU_TESS_COMBINE, combineCallback);
            //Glu.gluTessCallback(tess_obj_, Glu.GLU_TESS_ERROR, errorCallback);


            /*try
            {
                IntPtr retval = IntPtr.Zero;

                FT_Outline_Funcs outline_funcs = new FT_Outline_Funcs();
                outline_funcs.move_to = FT_Outline_MoveToFunc;
                outline_funcs.line_to = FT_Outline_LineToFunc;
                outline_funcs.conic_to = FT_Outline_ConicToFunc;
                outline_funcs.cubic_to = FT_Outline_CubicToFunc;

               // Glu.gluTessBeginPolygon(tess_obj_, 0);


                FT.FT_Outline_Decompose(ref glyphrec.outline, ref outline_funcs, retval);

                Gl.glTranslatef(glyphrec.bitmap.width, 0, 0);

                     if ( contour_open_ )
                        Glu.gluTessEndContour( tess_obj_ );                 

                Glu.gluTessEndPolygon( tess_obj_ );

            }
            catch (SystemException ex)
            {
                System.Console.WriteLine("Error compiling Outline List " + ex.Message);
            }*/

           // Gl.glEndList();
        //}

        private FT_Vector last_vector_;

        public int FT_Outline_MoveToFunc(ref FT_Vector to, IntPtr user) 
        {
              if ( contour_open_ ) 
              {
                    Glu.gluTessEndContour( tess_obj_ );
              }
             
             last_vector_ = to;
             
             Glu.gluTessBeginContour(tess_obj_ );             
             contour_open_ = true;

            return 0;  
        }

        public int FT_Outline_LineToFunc(ref FT_Vector to, IntPtr user) 
        {
            last_vector_ = to;            
             
//              vertex->v_[X] *= filled->vector_scale_;
//              vertex->v_[Y] *= filled->vector_scale_;
 
               
    //    Glu.gluTessVertex(tess_obj_, vertex->v_, vertex );
    
        //filled->vertices_.push_back( vertex );
  

            return 0; 
        }

        public int FT_Outline_ConicToFunc(ref FT_Vector control, ref FT_Vector to, IntPtr user) 
        { 
             // This is crude: Step off conics with a fixed number of increments
            /*VertexInfo to_vertex( to );
            1662     VertexInfo control_vertex( control );
            1663 
            1664     double b[2], c[2], d[2], f[2], df[2], d2f[2];
            1665 
            1666     b[X] = filled->last_vertex_.v_[X] - 2 * control_vertex.v_[X] +
            1667       to_vertex.v_[X];
            1668     b[Y] = filled->last_vertex_.v_[Y] - 2 * control_vertex.v_[Y] +
            1669       to_vertex.v_[Y];
            1670 
            1671     c[X] = -2 * filled->last_vertex_.v_[X] + 2 * control_vertex.v_[X];
            1672     c[Y] = -2 * filled->last_vertex_.v_[Y] + 2 * control_vertex.v_[Y];
            1673 
            1674     d[X] = filled->last_vertex_.v_[X];
            1675     d[Y] = filled->last_vertex_.v_[Y];
            1676 
            1677     f[X] = d[X];
            1678     f[Y] = d[Y];
            1679     df[X] = c[X] * filled->delta_ + b[X] * filled->delta2_;
            1680     df[Y] = c[Y] * filled->delta_ + b[Y] * filled->delta2_;
            1681     d2f[X] = 2 * b[X] * filled->delta2_;
            1682     d2f[Y] = 2 * b[Y] * filled->delta2_;
            1683 
            1684     for ( unsigned int i = 0; i < filled->tessellation_steps_-1; i++ ) {
            1685 
            1686       f[X] += df[X];
            1687       f[Y] += df[Y];
            1688 
            1689       VertexInfo* vertex = new VertexInfo( f );
            1690 
            1691       vertex->v_[X] *= filled->vector_scale_;
            1692       vertex->v_[Y] *= filled->vector_scale_;
            1693 
            1694       filled->vertices_.push_back( vertex );
            1695 
            1696       gluTessVertex( filled->tess_obj_, vertex->v_, vertex );
            1697 
            1698       df[X] += d2f[X];
            1699       df[Y] += d2f[Y];
            1700     }
            1701 
            1702     VertexInfo* vertex = new VertexInfo( to );
            1703 
            1704     vertex->v_[X] *= filled->vector_scale_;
            1705     vertex->v_[Y] *= filled->vector_scale_;
            1706 
            1707     filled->vertices_.push_back( vertex );
            1708 
            1709     gluTessVertex( filled->tess_obj_, vertex->v_, vertex );
            1710 
            1711     filled->last_vertex_ = to_vertex;
           */

            return 0; 
        }

        public int FT_Outline_CubicToFunc(ref FT_Vector control1, ref FT_Vector control2, ref FT_Vector to, IntPtr user)
        {
  /* VertexInfo to_vertex( to );
 1722     VertexInfo control1_vertex( control1 );
 1723     VertexInfo control2_vertex( control2 );
 1724 
 1725     double a[2], b[2], c[2], d[2], f[2], df[2], d2f[2], d3f[2];
 1726 
 1727     a[X] = -filled->last_vertex_.v_[X] + 3 * control1_vertex.v_[X]
 1728       -3 * control2_vertex.v_[X] + to_vertex.v_[X];
 1729     a[Y] = -filled->last_vertex_.v_[Y] + 3 * control1_vertex.v_[Y]
 1730       -3 * control2_vertex.v_[Y] + to_vertex.v_[Y];
 1731 
 1732     b[X] = 3 * filled->last_vertex_.v_[X] - 6 * control1_vertex.v_[X] +
 1733       3 * control2_vertex.v_[X];
 1734     b[Y] = 3 * filled->last_vertex_.v_[Y] - 6 * control1_vertex.v_[Y] +
 1735       3 * control2_vertex.v_[Y];
 1736 
 1737     c[X] = -3 * filled->last_vertex_.v_[X] + 3 * control1_vertex.v_[X];
 1738     c[Y] = -3 * filled->last_vertex_.v_[Y] + 3 * control1_vertex.v_[Y];
 1739 
 1740     d[X] = filled->last_vertex_.v_[X];
 1741     d[Y] = filled->last_vertex_.v_[Y];
 1742 
 1743     f[X] = d[X];
 1744     f[Y] = d[Y];
 1745     df[X] = c[X] * filled->delta_ + b[X] * filled->delta2_
 1746       + a[X] * filled->delta3_;
 1747     df[Y] = c[Y] * filled->delta_ + b[Y] * filled->delta2_
 1748       + a[Y] * filled->delta3_;
 1749     d2f[X] = 2 * b[X] * filled->delta2_ + 6 * a[X] * filled->delta3_;
 1750     d2f[Y] = 2 * b[Y] * filled->delta2_ + 6 * a[Y] * filled->delta3_;
 1751     d3f[X] = 6 * a[X] * filled->delta3_;
 1752     d3f[Y] = 6 * a[Y] * filled->delta3_;
 1753 
 1754     for ( unsigned int i = 0; i < filled->tessellation_steps_-1; i++ ) {
 1755 
 1756       f[X] += df[X];
 1757       f[Y] += df[Y];
 1758 
 1759       VertexInfo* vertex = new VertexInfo( f );
 1760 
 1761       vertex->v_[X] *= filled->vector_scale_;
 1762       vertex->v_[Y] *= filled->vector_scale_;
 1763 
 1764       filled->vertices_.push_back( vertex );
 1765 
 1766       gluTessVertex( filled->tess_obj_, vertex->v_, vertex );
 1767 
 1768       df[X] += d2f[X];
 1769       df[Y] += d2f[Y];
 1770       d2f[X] += d3f[X];
 1771       d2f[Y] += d3f[Y];
 1772     }
 1773 
 1774     VertexInfo* vertex = new VertexInfo( to );
 1775 
 1776     vertex->v_[X] *= filled->vector_scale_;
 1777     vertex->v_[Y] *= filled->vector_scale_;
 1778 
 1779     filled->vertices_.push_back( vertex );
 1780 
 1781     gluTessVertex( filled->tess_obj_, vertex->v_, vertex );
 1782 
 1783     filled->last_vertex_ = to_vertex;*/


            return 0;
        }

        public void vertexCallback( [In] IntPtr vertexData )
       {
          Gl.glVertex3dv(vertexData);
        }

        public void beginCallback(int which)
        {
            Gl.glBegin(which);
        }

        public void endCallback()
        {
            Gl.glEnd();
        }

        public void combineCallback([In] double[] coordinates, [In] IntPtr[] vertexData, [In] float[] weight, [Out] IntPtr[] outData)
        {
            //(void)vertex_data;
            //(void)weight;
            ////    cerr << "called combine" << endl;
            //VertexInfo* vertex = new VertexInfo( coords );
            //*out_data = vertex;
        }

        public void errorCallback(int error_code)
        {
            System.Console.WriteLine("GL Error during tesselation: " + error_code.ToString());
        }

		

        private void CompileCharacter(FT_FaceRec face, int c)
        {            			
			
            //We first convert the number index to a character index
            uint index = FT.FT_Get_Char_Index(faceptr, (uint)c);

            string sError = "";
            if (index == 0) sError = "No Glyph";

            //Here we load the actual glyph for the character
            int ret = FT.FT_Load_Glyph(faceptr, index, FT.FT_LOAD_RENDER);
            if (ret != 0)
            {
                Report("Load_Glyph failed for character " + c.ToString());
            }

            FT_GlyphSlotRec glyphrec = (FT_GlyphSlotRec)Marshal.PtrToStructure(face.glyph, typeof(FT_GlyphSlotRec));

            RenderAsTexture(glyphrec,c);           
            //RenderAsOutline(glyphrec,c);            
        }

        /// <summary>
        /// Dispose of the font
        /// </summary>
        public void Dispose()
        {
            ftClearFont();
            // Dispose of these as we don't need
            if (faceptr != IntPtr.Zero)
            {
                FT.FT_Done_Face(faceptr);
                faceptr = IntPtr.Zero;
            }
			          
        }

		/// <summary>
		/// Dispose of the FreeType library
		/// </summary>
		public static void DisposeFreeType()
		{
            FT.FT_Done_FreeType(FreeType.libptr);
		}
		
        /// <summary>
        /// Clear all OpenGL-related structures.
        /// </summary>
        public void ftClearFont()
        {

            if (list_base > 0)
            Gl.glDeleteLists(list_base, max_chars);

            if (textures!=null)
                Gl.glDeleteTextures(max_chars, textures);

            textures = null;
            extent_x = null;
        }

        /// <summary>
        /// Return the horizontal extent (width),in pixels, of a given font string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual float ftExtent(ref string text)
        {			
            int ret = 0;
            for (int c = 0; c < text.Length; c++)
                ret += extent_x[text[c]];
            return ret;
        }

		/// <summary>
		/// Return the Glyph offsets for the first character in "text"
		/// </summary>
		public FTGlyphOffset ftGetGlyphOffset(Char glyphchar)
		{
			return offsets[glyphchar];
		}
		
        /// <summary>
        /// Initialise the OpenGL state necessary fo rendering the font properly
        /// </summary>
        public void ftBeginFont()
        {
            int font = list_base;

            //Prepare openGL for rendering the font characters      
            Gl.glPushAttrib(Gl.GL_LIST_BIT | Gl.GL_CURRENT_BIT | Gl.GL_ENABLE_BIT | Gl.GL_TRANSFORM_BIT | Gl.GL_TEXTURE_BIT);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glEnable(Engine.Renderer.GlTextureMode);
            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glAlphaFunc(Gl.GL_GREATER, 0);
            Gl.glListBase(font);            
        }

        #region ftWrite(string text)
        /// <summary>
        ///     Custom GL "write" routine.
        /// </summary>
        /// <param name="text">
        ///     The text to print.
        /// </param>
        public void ftWrite(string text, ref int gllist)
        {
            if ((text == "") || (text == null)) return;

            // Scale to fit on Projection rendering screen with default coordinates                        
            Gl.glPushMatrix();

            switch (FT_ALIGN)
            {
                case FTFontAlign.FT_ALIGN_CENTERED:
                    Gl.glTranslatef(-ftExtent(ref text) / 2, 0, 0);
                    break;

                case FTFontAlign.FT_ALIGN_RIGHT:
                    Gl.glTranslatef(-ftExtent(ref text), 0, 0);
                    break;

                //By default it is left-aligned, so there's no need to bother translating by 0.
            }


            //Render
            if ((gllist != 0))
            {
                Gl.glCallList(gllist);
            }
            else
            {
                for (int i = 0; i < text.Length; i++)
                    CheckGlyph((int)text[i]);

                gllist = Gl.glGenLists(1);   
                Gl.glNewList(gllist, Gl.GL_COMPILE);

                //byte[] textbytes = new byte[text.Length];

                if (kerning)
                {
                    FT_Vector kern = new FT_Vector();

                    for (int i = 0; i < text.Length; i++)
                    {                        
                        if (i>0)
                        {
                            FT.FT_Get_Kerning(faceptr, (uint)text[i - 1], (uint)text[i], (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT, out kern);
                            Gl.glTranslatef(kern.x >> 6,0,0);
                        }

                        Gl.glCallLists(1, Gl.GL_UNSIGNED_INT, (uint)text[i]);
                    }
                }
                else
                    Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_SHORT, text);                             

                Gl.glEndList();

                //textbytes = null;
            }
            //textbytes[i] = (byte)text[i];
            //Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, textbytes);            
            Gl.glPopMatrix();
        }
        #endregion

        public uint GetChar(uint index)
        {
            return FT.FT_Get_Char_Index(faceptr, index);
        }

        #region ftWrite(string text)
        /// <summary>
        ///     Custom GL "write" routine. Slow version
        /// </summary>
        /// <param name="text">
        ///     The text to print.
        /// </param>
        public void ftWrite(string text)
        {
            if ((text == "") || (text == null)) return;

            // Scale to fit on Projection rendering screen with default coordinates                        
            Gl.glPushMatrix();

            switch (FT_ALIGN)
            {
                case FTFontAlign.FT_ALIGN_CENTERED:
                    Gl.glTranslatef(-ftExtent(ref text) / 2, 0, 0);
                    break;

                case FTFontAlign.FT_ALIGN_RIGHT:
                    Gl.glTranslatef(-ftExtent(ref text), 0, 0);
                    break;

                //By default it is left-aligned, so there's no need to bother translating by 0.
            }


            //Check Glyphs
            for (int i = 0; i < text.Length; i++)            
                CheckGlyph((int)text[i]);

            //Render


            if (kerning)
            {
                FT_Vector kern = new FT_Vector();

                for (int i = 0; i < text.Length; i++)
                {
                    if (i > 0)
                    {
                        FT.FT_Get_Kerning(faceptr, GetChar(text[i - 1]), GetChar(text[i]), (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT, out kern);
                        Gl.glTranslatef(kern.x >> 6, 0, 0);
                    }

                    Gl.glCallLists(1, Gl.GL_UNSIGNED_INT, (uint)text[i]);
                }
            }
            else
                Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_SHORT, text);

            Gl.glPopMatrix();
        }
        #endregion

        /// <summary>
        /// Restore the OpenGL state to what it was prior
        /// to starting to draw the font
        /// </summary>
        public void ftEndFont()
        {
            //Restore openGL state
            //Gl.glPopMatrix();
            Gl.glPopAttrib();          
        }
    }
}
