// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004 Novell, Inc. (http://www.novell.com)
//
// Author:
//	Ravindra (rkumar@novell.com)
//


// COMPLETE


using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
	[DefaultProperty ("Text")]
	[DesignTimeVisible (false)]
	[ToolboxItem (false)]
	public class ColumnHeader : Component, ICloneable
	{
		#region Instance Variables
		private StringFormat format = new StringFormat ();
		private string text = "ColumnHeader";
		private HorizontalAlignment text_alignment = HorizontalAlignment.Left;
		private int width = ThemeEngine.Current.ListViewDefaultColumnWidth;
#if NET_2_0
		private int image_index = -1;
		private string image_key = String.Empty;
		private string name = String.Empty;
		private object tag;
#endif

		// internal variables
		internal Rectangle column_rect = Rectangle.Empty;
		internal bool pressed = false;
		internal ListView owner;
		#endregion	// Instance Variables

		#region Internal Constructor
		internal ColumnHeader (ListView owner, string text,
				       HorizontalAlignment alignment, int width)
		{
			this.owner = owner;
			this.text = text;
			this.width = width;
			this.text_alignment = alignment;
			CalcColumnHeader ();
		}

#if NET_2_0
		internal ColumnHeader (string key, string text, int width, HorizontalAlignment textAlign)
		{
			Name = key;
			Text = text;
			this.width = width;
			this.text_alignment = textAlign;
			CalcColumnHeader ();
		}
#endif
		#endregion	// Internal Constructor

		#region Public Constructors
		public ColumnHeader () { }

#if NET_2_0
		public ColumnHeader (int imageIndex)
		{
			ImageIndex = imageIndex;
		}

		public ColumnHeader (string imageKey)
		{
			ImageKey = imageKey;
		}
#endif
		#endregion	// Public Constructors

		#region Private Internal Methods Properties
		// Since this class inherits from MarshalByRef,
		// we can't do ColumnHeader.column_rect.XXX. Hence,
		// we have some of the following properties to work around CS0197.
		internal bool Pressed {
			get { return this.pressed; }
		}

		internal int X {
			get { return this.column_rect.X; }
			set { this.column_rect.X = value; }
		}

		internal int Y {
			get { return this.column_rect.Y; }
			set { this.column_rect.Y = value; }
		}

		internal int Wd {
			get { return this.column_rect.Width; }
			set { this.column_rect.Width = value; }
		}

		internal int Ht {
			get { return this.column_rect.Height; }
			set { this.column_rect.Height = value; }
		}

		internal Rectangle Rect {
			get { return this.column_rect; }
		}

		internal StringFormat Format {
			get { return this.format; }
		}

		internal void CalcColumnHeader ()
		{			
			if (text_alignment == HorizontalAlignment.Center)
				format.Alignment = StringAlignment.Center;
			else if (text_alignment == HorizontalAlignment.Right)
				format.Alignment = StringAlignment.Far;
			else
				format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
			format.Trimming = StringTrimming.EllipsisWord;
			// text is wrappable only in LargeIcon and SmallIcon views
			format.FormatFlags = StringFormatFlags.NoWrap;

			if (width >= 0) {
				this.column_rect.Width = width;
				if (owner != null)
					this.column_rect.Height = owner.Font.Height + 5 ;
				else
					this.column_rect.Height = ThemeEngine.Current.DefaultFont.Height + 5;
			}
			else if (this.Index != -1)
				this.column_rect.Size = owner.GetChildColumnSize (this.Index);
			else
				this.column_rect.Size = Size.Empty;
		}
		#endregion	// Private Internal Methods Properties

		#region Public Instance Properties
#if NET_2_0
		public int ImageIndex {
			get {
				return image_index;
			}
			set {
				if (value < -1)
					throw new ArgumentOutOfRangeException ("value");

				image_index = value;
				image_key = String.Empty;
			}
		}

		public string ImageKey {
			get {
				return image_key;
			}
			set {
				image_key = value == null ? String.Empty : value;
				image_index = -1;
			}
		}

		public ImageList ImageList {
			get {
				if (owner == null)
					return null;

				return owner.SmallImageList;
			}
		}
#endif

		[Browsable (false)]
		public int Index {
			get {
				if (owner != null && owner.Columns != null
				    && owner.Columns.Contains (this)) {
					return owner.Columns.IndexOf (this);
				}
				return -1;
			}
		}

		[Browsable (false)]
		public ListView ListView {
			get { return owner; }
		}

#if NET_2_0
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}

		[LocalizableAttribute (false)]
		[BindableAttribute (true)]
		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
#endif

		[Localizable (true)]
		public string Text {
			get { return text; }
			set {
				text = value;
				if (owner != null)
					owner.Redraw (true);
			}
		}

		[DefaultValue (HorizontalAlignment.Left)]
		[Localizable (true)]
		public HorizontalAlignment TextAlign {
			get { return text_alignment; }
			set {
				text_alignment = value;
				if (owner != null)
					owner.Redraw (true);
			}
		}

		[DefaultValue (60)]
		[Localizable (true)]
		public int Width {
			get { return width; }
			set {
				width = value;
				if (owner != null)
					owner.Redraw (true);
			}
		}
		#endregion // Public Instance Properties

		#region Public Methods
		public object Clone ()
		{
			ColumnHeader columnHeader = new ColumnHeader ();
			columnHeader.text = text;
			columnHeader.text_alignment = text_alignment;
			columnHeader.width = width;
			columnHeader.owner = owner;
			columnHeader.format = (StringFormat) Format.Clone ();
			columnHeader.column_rect = Rectangle.Empty;
			return columnHeader;
		}

		public override string ToString ()
		{
			return string.Format ("ColumnHeader: Text: {0}", text);
		}
		#endregion // Public Methods

		#region Protected Methods
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}
		#endregion // Protected Methods
	}
}
