﻿using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MabiWorld.PropertyEditing
{
	/// <summary>
	/// Provides a collection editor with an event that is raised when
	/// the collection or one of its non-collection properties changed.
	/// </summary>
	public class NotifyingCollectionEditor : CollectionEditor
	{
		/// <summary>
		/// Raised when the collection changes.
		/// </summary>
		public static event EventHandler<CollectionChangedEventArgs> CollectionChanged;

		/// <summary>
		/// Raised when a property in the collection changes.
		/// </summary>
		public static event EventHandler<PropertyValueChangedEventArgs> CollectionPropertyChanged;

		/// <summary>
		/// Raised when the collection editor form is closed.
		/// </summary>
		public static event FormClosedEventHandler FormClosed;

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="type"></param>
		public NotifyingCollectionEditor(Type type)
			: base(type)
		{
		}

		/// <summary>
		/// Creates new collection form and hooks up events.
		/// </summary>
		/// <returns></returns>
		protected override CollectionForm CreateCollectionForm()
		{
			var form = base.CreateCollectionForm();
			var table = ((form as Form).Controls[0] as TableLayoutPanel);

			if (table != null)
			{
				// Hook up property changed event
				if (table.Controls[5] is PropertyGrid propertyGrid)
					propertyGrid.PropertyValueChanged += (sender, args) => CollectionPropertyChanged?.Invoke(this, args);

				// Hook up collection changed event to add and remove buttons
				if (table.Controls[1].Controls[0] is Button addButton)
					addButton.Click += this.OnAdd;
				if (table.Controls[1].Controls[1] is Button removeButton)
					removeButton.Click += this.OnRemove;

				// Hook up form closed event
				form.FormClosed += (sender, args) => FormClosed?.Invoke(this, args);
			}

			return form;
		}

		/// <summary>
		/// Called when an object is added to the collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAdd(object sender, EventArgs e)
		{
			CollectionChanged?.Invoke(this, new CollectionChangedEventArgs(this.Context, CollectionChangeType.Add));
		}

		/// <summary>
		/// Called when an object is added to the collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRemove(object sender, EventArgs e)
		{
			CollectionChanged?.Invoke(this, new CollectionChangedEventArgs(this.Context, CollectionChangeType.Remove));
		}
	}

	/// <summary>
	/// Arguments for the event that a collection property changed.
	/// </summary>
	public class CollectionChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Returns the context in which the collection changed.
		/// </summary>
		public ITypeDescriptorContext Context { get; }

		public CollectionChangeType Type { get; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="type"></param>
		public CollectionChangedEventArgs(ITypeDescriptorContext context, CollectionChangeType type)
		{
			this.Context = context;
			this.Type = type;
		}
	}

	/// <summary>
	/// Specifies how a collection has changed.
	/// </summary>
	public enum CollectionChangeType
	{
		/// <summary>
		/// An item was added.
		/// </summary>
		Add,

		/// <summary>
		/// An item was removed.
		/// </summary>
		Remove,
	}
}
