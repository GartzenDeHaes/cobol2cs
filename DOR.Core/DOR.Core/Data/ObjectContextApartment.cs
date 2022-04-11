using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

using DOR.Core.Threading;

namespace DOR.Core.Data
{
	public class ObjectContextApartment : ApartmentThreadedObject, IObjectContextApartment
	{
		private ObjectContext _ctx;

		public ObjectContextApartment(EntityConnection connection)
		{
			_ctx = new ObjectContext(connection);
		}

		public ObjectContextApartment(string connectionString)
		{
			_ctx = new ObjectContext(connectionString);
		}

		protected ObjectContextApartment(EntityConnection connection, string defaultContainerName)
		{
			_ctx = new ObjectContext(connection);
			_ctx.DefaultContainerName = defaultContainerName;
		}

		protected ObjectContextApartment(string connectionString, string defaultContainerName)
		{
			_ctx = new ObjectContext(connectionString);
			_ctx.DefaultContainerName = defaultContainerName;
		}
		
		//
		// Summary:
		//     Gets the object state manager used by the object context to track object
		//     changes.
		//
		// Returns:
		//     The System.Data.Objects.ObjectStateManager used by this System.Data.Objects.ObjectContext.
		public ObjectStateManager ObjectStateManager 
		{
			get
			{
				return _ctx.ObjectStateManager;
			}
		}

		//
		// Summary:
		//     Gets the metadata workspace used by the object context.
		//
		// Returns:
		//     The System.Data.Metadata.Edm.MetadataWorkspace object associated with this
		//     System.Data.Objects.ObjectContext.
		[CLSCompliant(false)]
		public MetadataWorkspace MetadataWorkspace 
		{
			get
			{
				return Dispatch<MetadataWorkspace>(_ctx, _ctx.GetType().GetProperty("MetadataWorkspace").GetGetMethod(), null);
			}
		}

		// Summary:
		//     Gets or sets the timeout value, in seconds, for all object context operations.
		//     A null value indicates that the default value of the underlying provider
		//     will be used.
		//
		// Returns:
		//     An System.Int32 value that is the timeout value, in seconds.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     The timeout value is less than 0.
		public int? CommandTimeout 
		{
			get { return _ctx.CommandTimeout; }
			set { _ctx.CommandTimeout = value; }
		}
		//
		// Summary:
		//     Gets the System.Data.Objects.ObjectContextOptions instance that contains
		//     options that affect the behavior of the System.Data.Objects.ObjectContext.
		//
		// Returns:
		//     The System.Data.Objects.ObjectContextOptions instance that contains options
		//     that affect the behavior of the System.Data.Objects.ObjectContext.
		public ObjectContextOptions ContextOptions 
		{
			get { return _ctx.ContextOptions; }
		}
		//
		// Summary:
		//     Gets or sets the default container name.
		//
		// Returns:
		//     A System.String that is the default container name.
		public string DefaultContainerName 
		{
			get { return _ctx.DefaultContainerName; }
			set { _ctx.DefaultContainerName = value; }
		}

		// Summary:
		//     Occurs when a new entity object is created from data in the data source as
		//     part of a query or load operation.
		public event ObjectMaterializedEventHandler ObjectMaterialized
		{
			add { _ctx.ObjectMaterialized += value; }
			remove { _ctx.ObjectMaterialized -= value; }
		}

		//
		// Summary:
		//     Occurs when changes are saved to the data source.
		public event EventHandler SavingChanges
		{
			add { _ctx.SavingChanges += value; }
			remove { _ctx.SavingChanges -= value; }
		}

		// Summary:
		//     Accepts all changes made to objects in the object context.
		public void AcceptAllChanges()
		{
			Dispatch(_ctx, "AcceptAllChanges");
		}
		//
		// Summary:
		//     Adds an object to the object context.
		//
		// Parameters:
		//   entitySetName:
		//     Represents the entity set name, which may optionally be qualified by the
		//     entity container name.
		//
		//   entity:
		//     The System.Object to add.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The entity parameter is null. -or-The entitySetName does not qualify.
		public void AddObject(string entitySetName, object entity)
		{
			Dispatch(_ctx, "AddObject", entitySetName, entity);
		}
		//
		// Summary:
		//     Sets the System.Data.Objects.ObjectStateEntry.CurrentValues property of the
		//     System.Data.Objects.ObjectStateEntry to match the property values of a supplied
		//     object.
		//
		// Parameters:
		//   entitySetName:
		//     The name of the entity set to which the object belongs.
		//
		//   currentEntity:
		//     The detached object that has property updates to apply to the original object.
		//
		// Type parameters:
		//   TEntity:
		//     The entity type of the object.
		//
		// Returns:
		//     The updated object.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     When entitySetName or current is null.
		//
		//   System.InvalidOperationException:
		//     When the System.Data.Metadata.Edm.EntitySet from entitySetName does not match
		//     the System.Data.Metadata.Edm.EntitySet of the object’s System.Data.EntityKey.-or-When
		//     the object is not in the System.Data.Objects.ObjectStateManager or it is
		//     in a System.Data.EntityState.Detached state.-or- The entity key of the supplied
		//     object is invalid.
		//
		//   System.ArgumentException:
		//     When entitySetName is an empty string.
		public TEntity ApplyCurrentValues<TEntity>(string entitySetName, TEntity currentEntity) where TEntity : class
		{
			return DispatchTemplated<TEntity>(_ctx, "ApplyCurrentValues", typeof(TEntity), new object[] { entitySetName, currentEntity });
		}
		//
		// Summary:
		//     Sets the System.Data.Objects.ObjectStateEntry.OriginalValues property of
		//     the System.Data.Objects.ObjectStateEntry to match the property values of
		//     a supplied object.
		//
		// Parameters:
		//   entitySetName:
		//     The name of the entity set to which the object belongs.
		//
		//   originalEntity:
		//     The detached object that has original values to apply to the object.
		//
		// Type parameters:
		//   TEntity:
		//     The type of the entity object.
		//
		// Returns:
		//     The updated object.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     When entitySetName or original is null.
		//
		//   System.InvalidOperationException:
		//     When the System.Data.Metadata.Edm.EntitySet from entitySetName does not match
		//     the System.Data.Metadata.Edm.EntitySet of the object’s System.Data.EntityKey.-or-When
		//     an System.Data.Objects.ObjectStateEntry for the object cannot be found in
		//     the System.Data.Objects.ObjectStateManager. -or-When the object is in an
		//     System.Data.EntityState.Added or a System.Data.EntityState.Detached state.-or-
		//     The entity key of the supplied object is invalid or has property changes.
		//
		//   System.ArgumentException:
		//     When entitySetName is an empty string.
		public TEntity ApplyOriginalValues<TEntity>(string entitySetName, TEntity originalEntity) where TEntity : class
		{
			return DispatchTemplated<TEntity>(_ctx, "ApplyOriginalValues", typeof(TEntity), new object[] { entitySetName, originalEntity });
		}
		//
		// Summary:
		//     Attaches an object or object graph to the object context when the object
		//     has an entity key.
		//
		// Parameters:
		//   entity:
		//     The object to attach.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The entity is null.
		//
		//   System.InvalidOperationException:
		//     Invalid entity key.
		public void Attach(IEntityWithKey entity)
		{
			Dispatch(_ctx, "Attach", entity);
		}
		//
		// Summary:
		//     Attaches an object or object graph to the object context in a specific entity
		//     set.
		//
		// Parameters:
		//   entitySetName:
		//     Represents the entity set name, which may optionally be qualified by the
		//     entity container name.
		//
		//   entity:
		//     The System.Object to attach.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The entity is null.
		//
		//   System.InvalidOperationException:
		//     Invalid entity set.-or-The object has a temporary key. -or-The object has
		//     an System.Data.EntityKey and the System.Data.Metadata.Edm.EntitySet does
		//     not match with the entity set passed in as an argument of the method.-or-The
		//     object does not have an System.Data.EntityKey and no entity set is provided.-or-Any
		//     object from the object graph has a temporary System.Data.EntityKey.-or-Any
		//     object from the object graph has an invalid System.Data.EntityKey (for example,
		//     values in the key do not match values in the object).-or-The entity set could
		//     not be found from a given entitySetName name and entity container name.-or-Any
		//     object from the object graph already exists in another state manager.
		public void AttachTo(string entitySetName, object entity)
		{
			Dispatch(_ctx, "AttachTo", entitySetName, entity);
		}
		//
		// Summary:
		//     Creates the database by using the current data source connection and the
		//     metadata in the System.Data.Metadata.Edm.StoreItemCollection.
		public void CreateDatabase()
		{
			Dispatch(_ctx, "CreateDatabase");
		}
		//
		// Summary:
		//     Generates a data definition langauge (DDL) script that creates schema objects
		//     (tables, primary keys, foreign keys) for the metadata in the System.Data.Metadata.Edm.StoreItemCollection.
		//
		// Returns:
		//     A DDL script that creates schema objects for the metadata in the System.Data.Metadata.Edm.StoreItemCollection.
		public string CreateDatabaseScript()
		{
			return Dispatch<string>(_ctx, "CreateDatabaseScript");
		}
		//
		// Summary:
		//     Creates the entity key for a specific object, or returns the entity key if
		//     it already exists.
		//
		// Parameters:
		//   entitySetName:
		//     The fully qualified name of the entity set to which the entity object belongs.
		//
		//   entity:
		//     The object for which the entity key is being retrieved. The object must implement
		//     System.Data.Objects.DataClasses.IEntityWithKey.
		//
		// Returns:
		//     The System.Data.EntityKey of the object.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     When either parameter is null.
		//
		//   System.ArgumentException:
		//     When entitySetName is empty.-or- When the type of the entity object does
		//     not exist in the entity set.-or-When the entitySetName is not fully qualified.
		//
		//   System.InvalidOperationException:
		//     When the entity key cannot be constructed successfully based on the supplied
		//     parameters.
		public EntityKey CreateEntityKey(string entitySetName, object entity)
		{
			return Dispatch<EntityKey>(_ctx, "CreateEntityKey", entitySetName, entity);
		}
		//
		// Summary:
		//     Creates and returns an instance of the requested type .
		//
		// Type parameters:
		//   T:
		//     Type of object to be returned.
		//
		// Returns:
		//     An instance of the requested type T, or an instance of a derived type that
		//     enables T to be used with the Entity Framework. The returned object is either
		//     an instance of the requested type or an instance of a derived type that enables
		//     the requested type to be used with the Entity Framework.
		public T CreateObject<T>() where T : class
		{
			return DispatchTemplated<T>(_ctx, "CreateObject", typeof(T));
		}
		//
		// Summary:
		//     Creates a new System.Data.Objects.ObjectSet<TEntity> instance that is used
		//     to query, add, modify, and delete objects of the specified entity type.
		//
		// Type parameters:
		//   TEntity:
		//     Entity type of the requested System.Data.Objects.ObjectSet<TEntity>.
		//
		// Returns:
		//     The new System.Data.Objects.ObjectSet<TEntity> instance.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     When the System.Data.Objects.ObjectContext.DefaultContainerName property
		//     is not set on the System.Data.Objects.ObjectContext.-or-When the specified
		//     type belongs to more than one entity set.
		public ObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class
		{
			return DispatchTemplated<ObjectSet<TEntity>>(_ctx, "CreateObjectSet", typeof(TEntity));
		}
		//
		// Summary:
		//     Creates a new System.Data.Objects.ObjectSet<TEntity> instance that is used
		//     to query, add, modify, and delete objects of the specified type and with
		//     the specified entity set name.
		//
		// Parameters:
		//   entitySetName:
		//     Name of the entity set for the returned System.Data.Objects.ObjectSet<TEntity>.
		//     The string must be qualified by the default container name if the System.Data.Objects.ObjectContext.DefaultContainerName
		//     property is not set on the System.Data.Objects.ObjectContext.
		//
		// Type parameters:
		//   TEntity:
		//     Entity type of the requested System.Data.Objects.ObjectSet<TEntity>.
		//
		// Returns:
		//     The new System.Data.Objects.ObjectSet<TEntity> instance.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     When the System.Data.Metadata.Edm.EntitySet from entitySetName does not match
		//     the System.Data.Metadata.Edm.EntitySet of the object’s System.Data.EntityKey.-or-When
		//     the System.Data.Objects.ObjectContext.DefaultContainerName property is not
		//     set on the System.Data.Objects.ObjectContext and the name is not qualified
		//     as part of the entitySetName parameter.-or-When the specified type belongs
		//     to more than one entity set.
		public ObjectSet<TEntity> CreateObjectSet<TEntity>(string entitySetName) where TEntity : class
		{
			return DispatchTemplated<ObjectSet<TEntity>>(_ctx, "CreateObjectSet", typeof(TEntity), entitySetName);
		}
		//
		// Summary:
		//     Generates an equivalent type that can be used with the Entity Framework for
		//     each type in the supplied enumeration.
		//
		// Parameters:
		//   types:
		public void CreateProxyTypes(IEnumerable<Type> types)
		{
			Dispatch(_ctx, "CreateProxyTypes", types);
		}
		//
		// Summary:
		//     Creates an System.Data.Objects.ObjectQuery<T> in the current object context
		//     by using the specified query string.
		//
		// Parameters:
		//   queryString:
		//     The query string to be executed.
		//
		//   parameters:
		//     Parameters to pass to the query.
		//
		// Type parameters:
		//   T:
		//     The entity type of the returned System.Data.Objects.ObjectQuery<T>.
		//
		// Returns:
		//     An System.Data.Objects.ObjectQuery<T> of the specified type.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The queryString or parameters parameter is null.
		public ObjectQuery<T> CreateQuery<T>(string queryString, params ObjectParameter[] parameters)
		{
			return DispatchTemplated<ObjectQuery<T>>(_ctx, "CreateQuery", typeof(T), queryString, parameters);
		}
		//
		// Summary:
		//     Checks if the database that is specified as the database in the current data
		//     source connection exists on the data source.
		//
		// Returns:
		//     true if the database exists.
		public bool DatabaseExists()
		{
			return _ctx.DatabaseExists();
		}
		//
		// Summary:
		//     Deletes the database that is specified as the database in the current data
		//     source connection.
		public void DeleteDatabase()
		{
			Dispatch(_ctx, "DeleteDatabase");
		}
		//
		// Summary:
		//     Marks an object for deletion.
		//
		// Parameters:
		//   entity:
		//     An object that specifies the entity to delete. The object can be in any state
		//     except System.Data.EntityState.Detached.
		public void DeleteObject(object entity)
		{
			Dispatch(_ctx, "DeleteObject", entity);
		}
		//
		// Summary:
		//     Removes the object from the object context.
		//
		// Parameters:
		//   entity:
		//     Object to be detached. Only the entity is removed; if there are any related
		//     objects that are being tracked by the same System.Data.Objects.ObjectStateManager,
		//     those will not be detached automatically.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The entity is null.
		//
		//   System.InvalidOperationException:
		//     The entity is not associated with this System.Data.Objects.ObjectContext
		//     (for example, was newly created and not associated with any context yet,
		//     or was obtained through some other context, or was already detached).
		public void Detach(object entity)
		{
			Dispatch(_ctx, "Detach", entity);
		}
		//
		// Summary:
		//     Ensures that System.Data.Objects.ObjectStateEntry changes are synchronized
		//     with changes in all objects that are tracked by the System.Data.Objects.ObjectStateManager.
		public void DetectChanges()
		{
			Dispatch(_ctx, "DetectChanges");
		}
		//
		// Summary:
		//     Releases the resources used by the object context.
		public override void Dispose()
		{
			Dispose(true);
		}
		//
		// Summary:
		//     Releases the resources used by the object context.
		//
		// Parameters:
		//   disposing:
		//     true to release both managed and unmanaged resources; false to release only
		//     unmanaged resources.
		/*virtual*/
		public override void Dispose(bool disposing)
		{
			if (null != _ctx)
			{
				Stop();
				_ctx.Dispose();
				_ctx = null;
			}
		}
		//
		// Summary:
		//     Executes a stored procedure or function that is defined in the data source
		//     and expressed in the conceptual model; discards any results returned from
		//     the function; and returns the number of rows affected by the execution.
		//
		// Parameters:
		//   functionName:
		//     The name of the stored procedure or function. The name can include the container
		//     name, such as <Container Name>.<Function Name>. When the default container
		//     name is known, only the function name is required.
		//
		//   parameters:
		//     An array of System.Data.Objects.ObjectParameter objects.
		//
		// Returns:
		//     The number of rows affected.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     function is null or empty -or-function is not found.
		//
		//   System.InvalidOperationException:
		//     The entity reader does not support this function.-or-There is a type mismatch
		//     on the reader and the function.
		public int ExecuteFunction(string functionName, params ObjectParameter[] parameters)
		{
			return DispatchSearch<int>(_ctx, "ExecuteFunction", functionName, parameters);
		}
		//
		// Summary:
		//     Executes a stored procedure or function that is defined in the data source
		//     and mapped in the conceptual model, with the specified parameters. Returns
		//     a typed System.Data.Objects.ObjectResult<T>.
		//
		// Parameters:
		//   functionName:
		//     The name of the stored procedure or function. The name can include the container
		//     name, such as <Container Name>.<Function Name>. When the default container
		//     name is known, only the function name is required.
		//
		//   parameters:
		//     An array of System.Data.Objects.ObjectParameter objects.
		//
		// Type parameters:
		//   TElement:
		//     The entity type of the System.Data.Objects.ObjectResult<T> returned when
		//     the function is executed against the data source. This type must implement
		//     System.Data.Objects.DataClasses.IEntityWithChangeTracker.
		//
		// Returns:
		//     An System.Data.Objects.ObjectResult<T> for the data that is returned by the
		//     stored procedure.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     function is null or empty -or-function is not found.
		//
		//   System.InvalidOperationException:
		//     The entity reader does not support this function.-or-There is a type mismatch
		//     on the reader and the function.
		[System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, params ObjectParameter[] parameters)
		{
			return DispatchTemplatedSearch<ObjectResult<TElement>>(_ctx, "ExecuteFunction", typeof(TElement), functionName, parameters);
		}
		//
		// Summary:
		//     Executes the given stored procedure or function that is defined in the data
		//     source and expressed in the conceptual model, with the specified parameters,
		//     and merge option. Returns a typed System.Data.Objects.ObjectResult<T>.
		//
		// Parameters:
		//   functionName:
		//     The name of the stored procedure or function. The name can include the container
		//     name, such as <Container Name>.<Function Name>. When the default container
		//     name is known, only the function name is required.
		//
		//   mergeOption:
		//     The System.Data.Objects.MergeOption to use when executing the query.
		//
		//   parameters:
		//     An array of System.Data.Objects.ObjectParameter objects.
		//
		// Type parameters:
		//   TElement:
		//     The entity type of the System.Data.Objects.ObjectResult<T> returned when
		//     the function is executed against the data source. This type must implement
		//     System.Data.Objects.DataClasses.IEntityWithChangeTracker.
		//
		// Returns:
		//     An System.Data.Objects.ObjectResult<T> for the data that is returned by the
		//     stored procedure.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     function is null or empty -or-function is not found.
		//
		//   System.InvalidOperationException:
		//     The entity reader does not support this function.-or-There is a type mismatch
		//     on the reader and the function.
		public ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, MergeOption mergeOption, params ObjectParameter[] parameters)
		{
			return DispatchTemplatedSearch<ObjectResult<TElement>>(_ctx, "ExecuteFunction", typeof(TElement), functionName, mergeOption, parameters);
		}
		//
		// Summary:
		//     Executes an arbitrary command directly against the data source using the
		//     existing connection.
		//
		// Parameters:
		//   commandText:
		//     The command to execute, in the native language of the data source.
		//
		//   parameters:
		//     An array of parameters to pass to the command.
		//
		// Returns:
		//     The number of rows affected.
		public int ExecuteStoreCommand(string commandText, params object[] parameters)
		{
			return Dispatch<int>(_ctx, "ExecuteStoreCommand", commandText, parameters);
		}
		//
		// Summary:
		//     Executes a query directly against the data source that returns a sequence
		//     of typed results.
		//
		// Parameters:
		//   commandText:
		//     The command to execute, in the native language of the data source.
		//
		//   parameters:
		//     An array of parameters to pass to the command.
		//
		// Type parameters:
		//   TElement:
		//
		// Returns:
		//     An enumeration of objects of type TResult.
		[System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public ObjectResult<TElement> ExecuteStoreQuery<TElement>(string commandText, params object[] parameters)
		{
			return DispatchTemplated<ObjectResult<TElement>>(_ctx, "ExecuteStoreQuery", typeof(TElement), commandText, parameters);
		}
		//
		// Summary:
		//     Executes a query directly against the data source and returns a sequence
		//     of typed results. Specify the entity set and the merge option so that query
		//     results can be tracked as entities.
		//
		// Parameters:
		//   commandText:
		//     The command to execute, in the native language of the data source.
		//
		//   entitySetName:
		//     The entity set of the TResult type. If an entity set name is not provided,
		//     the results are not going to be tracked.
		//
		//   mergeOption:
		//     The System.Data.Objects.MergeOption to use when executing the query. The
		//     default is System.Data.Objects.MergeOption.AppendOnly.
		//
		//   parameters:
		//     An array of parameters to pass to the command.
		//
		// Type parameters:
		//   TEntity:
		//
		// Returns:
		//     An enumeration of objects of type TResult.
		public ObjectResult<TEntity> ExecuteStoreQuery<TEntity>(string commandText, string entitySetName, MergeOption mergeOption, params object[] parameters)
		{
			return DispatchTemplated<ObjectResult<TEntity>>(_ctx, "ExecuteStoreQuery", typeof(TEntity), commandText, entitySetName, mergeOption, parameters);
		}
		//
		// Summary:
		//     Returns an object that has the specified entity key.
		//
		// Parameters:
		//   key:
		//     The key of the object to be found.
		//
		// Returns:
		//     An System.Object that is an instance of an entity type.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The key parameter is null.
		//
		//   System.Data.ObjectNotFoundException:
		//     The object is not found in either the System.Data.Objects.ObjectStateManager
		//     or the data source.
		public object GetObjectByKey(EntityKey key)
		{
			return Dispatch<object>(_ctx, "GetObjectByKey", key);
		}
		//
		// Summary:
		//     Explicitly loads an object related to the supplied object by the specified
		//     navigation property and using the default merge option.
		//
		// Parameters:
		//   entity:
		//     The entity for which related objects are to be loaded.
		//
		//   navigationProperty:
		//     The name of the navigation property that returns the related objects to be
		//     loaded.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     When the entity is in a System.Data.EntityState.Detached, System.Data.EntityState.Added,
		//     or System.Data.EntityState.Deleted state,-or-When the entity is attached
		//     to another instance of System.Data.Objects.ObjectContext.
		public void LoadProperty(object entity, string navigationProperty)
		{
			Dispatch(_ctx, "LoadProperty", entity, navigationProperty);
		}
		//
		// Summary:
		//     Explicitly loads an object that is related to the supplied object by the
		//     specified LINQ query and by using the default merge option.
		//
		// Parameters:
		//   entity:
		//     The source object for which related objects are to be loaded.
		//
		//   selector:
		//     A LINQ expression that defines the related objects to be loaded.
		//
		// Type parameters:
		//   TEntity:
		//
		// Exceptions:
		//   System.ArgumentException:
		//     When selector does not supply a valid input parameter.
		//
		//   System.ArgumentNullException:
		//     When selector is null.
		//
		//   System.InvalidOperationException:
		//     When the entity is in a System.Data.EntityState.Detached, System.Data.EntityState.Added,
		//     or System.Data.EntityState.Deleted state,-or-When the entity is attached
		//     to another instance of System.Data.Objects.ObjectContext.
		public void LoadProperty<TEntity>(TEntity entity, Expression<Func<TEntity, object>> selector)
		{
			DispatchTemplated(_ctx, "LoadProperty", typeof(TEntity), entity, selector);
		}
		//
		// Summary:
		//     Explicitly loads an object that is related to the supplied object by the
		//     specified navigation property and using the specified merge option.
		//
		// Parameters:
		//   entity:
		//     The entity for which related objects are to be loaded.
		//
		//   navigationProperty:
		//     The name of the navigation property that returns the related objects to be
		//     loaded.
		//
		//   mergeOption:
		//     The System.Data.Objects.MergeOption value to use when you load the related
		//     objects.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     When the entity is in a System.Data.EntityState.Detached, System.Data.EntityState.Added,
		//     or System.Data.EntityState.Deleted state,-or-When the entity is attached
		//     to another instance of System.Data.Objects.ObjectContext.
		public void LoadProperty(object entity, string navigationProperty, MergeOption mergeOption)
		{
			Dispatch(_ctx, "LoadProperty", entity, navigationProperty, mergeOption);
		}
		//
		// Summary:
		//     Explicitly loads an object that is related to the supplied object by the
		//     specified LINQ query and by using the specified merge option.
		//
		// Parameters:
		//   entity:
		//     The source object for which related objects are to be loaded.
		//
		//   selector:
		//     A LINQ expression that defines the related objects to be loaded.
		//
		//   mergeOption:
		//     The System.Data.Objects.MergeOption value to use when you load the related
		//     objects.
		//
		// Type parameters:
		//   TEntity:
		//
		// Exceptions:
		//   System.ArgumentException:
		//     When selector does not supply a valid input parameter.
		//
		//   System.ArgumentNullException:
		//     When selector is null.
		//
		//   System.InvalidOperationException:
		//     When the entity is in a System.Data.EntityState.Detached, System.Data.EntityState.Added,
		//     or System.Data.EntityState.Deleted state,-or-When the entity is attached
		//     to another instance of System.Data.Objects.ObjectContext.
		public void LoadProperty<TEntity>(TEntity entity, Expression<Func<TEntity, object>> selector, MergeOption mergeOption)
		{
			DispatchTemplated(_ctx, "LoadProperty", typeof(TEntity), entity, selector, mergeOption);
		}
		//
		// Summary:
		//     Updates a collection of objects in the object context with data from the
		//     data source.
		//
		// Parameters:
		//   refreshMode:
		//     A System.Data.Objects.RefreshMode value that indicates whether property changes
		//     in the object context are overwritten with property values from the data
		//     source.
		//
		//   collection:
		//     An System.Collections.IEnumerable collection of objects to refresh.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     collection is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     refreshMode is not valid.
		//
		//   System.ArgumentException:
		//     collection is empty. -or- An object is not attached to the context.
		public void Refresh(RefreshMode refreshMode, IEnumerable collection)
		{
			Dispatch(_ctx, "Refresh", refreshMode, collection);
		}
		//
		// Summary:
		//     Updates an object in the object context with data from the data source.
		//
		// Parameters:
		//   refreshMode:
		//     One of the System.Data.Objects.RefreshMode values that specifies which mode
		//     to use for refreshing the System.Data.Objects.ObjectStateManager.
		//
		//   entity:
		//     The object to be refreshed.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     collection is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     refreshMode is not valid.
		//
		//   System.ArgumentException:
		//     collection is empty. -or- An object is not attached to the context.
		public void Refresh(RefreshMode refreshMode, object entity)
		{
			Dispatch(_ctx, "Refresh", refreshMode, entity);
		}
		//
		// Summary:
		//     Persists all updates to the data source and resets change tracking in the
		//     object context.
		//
		// Returns:
		//     The number of objects in an System.Data.EntityState.Added, System.Data.EntityState.Modified,
		//     or System.Data.EntityState.Deleted state when System.Data.Objects.ObjectContext.SaveChanges()
		//     was called.
		//
		// Exceptions:
		//   System.Data.OptimisticConcurrencyException:
		//     An optimistic concurrency violation has occurred in the data source.
		public int SaveChanges()
		{
			return DispatchSearch<int>(_ctx, "SaveChanges");
		}
		//
		// Summary:
		//     Persists all updates to the data source with the specified System.Data.Objects.SaveOptions.
		//
		// Parameters:
		//   options:
		//     A System.Data.Objects.SaveOptions value that determines the behavior of the
		//     operation.
		//
		// Returns:
		//     The number of objects in an System.Data.EntityState.Added, System.Data.EntityState.Modified,
		//     or System.Data.EntityState.Deleted state when System.Data.Objects.ObjectContext.SaveChanges()
		//     was called.
		//
		// Exceptions:
		//   System.Data.OptimisticConcurrencyException:
		//     An optimistic concurrency violation has occurred.
		public virtual int SaveChanges(SaveOptions options)
		{
			return Dispatch<int>(_ctx, "SaveChanges", options);
		}
		//
		// Summary:
		//     Translates a System.Data.Common.DbDataReader that contains rows of entity
		//     data to objects of the requested entity type.
		//
		// Parameters:
		//   reader:
		//     The System.Data.Common.DbDataReader that contains entity data to translate
		//     into entity objects.
		//
		// Type parameters:
		//   TElement:
		//
		// Returns:
		//     An enumeration of objects of type TResult.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     When reader is null.
		public ObjectResult<TElement> Translate<TElement>(DbDataReader reader)
		{
			return DispatchTemplated<ObjectResult<TElement>>(_ctx, "Translate", typeof(TElement), reader);
		}
		//
		// Summary:
		//     Translates a System.Data.Common.DbDataReader that contains rows of entity
		//     data to objects of the requested entity type, in a specific entity set, and
		//     with the specified merge option.
		//
		// Parameters:
		//   reader:
		//     The System.Data.Common.DbDataReader that contains entity data to translate
		//     into entity objects.
		//
		//   entitySetName:
		//     The entity set of the TResult type.
		//
		//   mergeOption:
		//     The System.Data.Objects.MergeOption to use when translated objects are added
		//     to the object context. The default is System.Data.Objects.MergeOption.AppendOnly.
		//
		// Type parameters:
		//   TEntity:
		//
		// Returns:
		//     An enumeration of objects of type TResult.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     When reader is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     When the supplied mergeOption is not a valid System.Data.Objects.MergeOption
		//     value.
		//
		//   System.InvalidOperationException:
		//     When the supplied entitySetName is not a valid entity set for the TResult
		//     type.
		public ObjectResult<TEntity> Translate<TEntity>(DbDataReader reader, string entitySetName, MergeOption mergeOption)
		{
			return DispatchTemplated<ObjectResult<TEntity>>(_ctx, "Translate", typeof(TEntity), reader, entitySetName, mergeOption);
		}
	}
}
