using System;
using System.Runtime.Serialization;

namespace Avista.ESB.Utilities.Components
{
    [Serializable]
    public class ComponentBase : IComponent, ISerializable
    {
        /// <summary>
        /// Flag that indicates whether or not the object has been disposed. 
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The name of the component.
        /// </summary>
        private string name = null;

        /// <summary>
        /// The unique identifier for the component.
        /// </summary>
        private Guid componentId = Guid.NewGuid();

        /// <summary>
        /// Constructor for the component. All     components are named
        /// to facilitate lookup of configuration information.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        public ComponentBase(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of ComponentBase with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected ComponentBase(SerializationInfo info, StreamingContext context)
        {
            name = (string)info.GetValue("name", typeof(string));
        }

        /// <summary>
        /// Serializes the ComponentBase instance.
        /// </summary>
        /// <param name="info">The SerializationInfo to populate with data.</param>
        /// <param name="context">The context for the serialization.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", name);
        }

        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The unique identifier for the component.
        /// </summary>
        public Guid ComponentId
        {
            get { return componentId; }
        }

        /// <summary>
        /// Refreshes the configuration of the component.
        /// </summary>
        public virtual void RefreshConfiguration()
        {
        }

        /// <summary>
        /// Finalizer for the class.
        /// </summary>
        ~ComponentBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the object and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   true to release both managed and unmanaged resources;
        ///   false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Dispose managed resources
                if (disposing)
                {
                }
                // Dispose unmanaged resources
            }
            disposed = true;
        }

        /// <summary>
        /// Check to see if the object has been disposed.
        /// </summary>
        protected void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}
