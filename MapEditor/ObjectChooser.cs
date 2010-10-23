using Engine;
using System;
using Gtk;

namespace MapEditor
{


	public partial class ObjectChooser : Gtk.Window //, IEditorListener
	{

		public ObjectChooser (EditorModel model) : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			
			Deletable = false; //Why doesn't this work?
			
			//Prevent this window from closing
			DeleteEvent += delegate(object o, Gtk.DeleteEventArgs args) {
				args.RetVal = true;
			};
			
			TreeViewColumn nameColumn = new TreeViewColumn();
			CellRendererText nameCell = new CellRendererText();
			nameColumn.Title = "Object name";
			nameColumn.PackStart(nameCell, true);
			nameColumn.AddAttribute(nameCell, "text", 0);
			
			listObjects.AppendColumn(nameColumn);
			
			
			ListStore objectStore = new ListStore(typeof(string));
			listObjects.Model = objectStore;
			
			//Set up object list
			foreach (string o in model.ResourceManager.Objects)
			{
				objectStore.AppendValues(o);
			}
			
			listObjects.CursorChanged += delegate(object sender, EventArgs e) {
				TreeIter iter;
				if (listObjects.Selection.GetSelected(out iter))
				{
					model.CurrentObject = (objectStore.GetValue(iter, 0) as string);
				}
			};

		}
		
		/*public void ModelChanged(EditorModel.VariableName v, object oldValue, object newValue)
		{
			switch (v)
			{
				case EditorModel.VariableName.
			}
		}*/
	}
}
