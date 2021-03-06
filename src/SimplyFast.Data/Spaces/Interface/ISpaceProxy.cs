﻿using System;
using System.Collections.Generic;

namespace SimplyFast.Data.Spaces.Interface
{
    /// <summary>
    /// Convinient interface for space access
    /// Treat like IDbConnection
    /// </summary>
    public interface ISpaceProxy: IDisposable
    {
        /// <summary>
        /// Begin transaction or nested transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits transaction or nested transaction
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Commits transaction or nested transaction
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Get's current transaction nested level
        /// </summary>
        int TransactionCount { get; }

        /// <summary>
        /// read entity from space if exists, otherwise return default
        /// </summary>
        T TryRead<T>(IQuery<T> query);
        /// <summary>
        /// read and remove entity from space if exists, otherwise return default
        /// </summary>
        T TryTake<T>(IQuery<T> query);
        /// <summary>
        /// read entity from space if exists, othewise waits for entity. Use IDisposable to stop waiting
        /// </summary>
        IDisposable Read<T>(IQuery<T> query, Action<T> callback);
        /// <summary>
        /// read and remove entity from space if exists, othewise waits for entity. Use IDisposable to stop waiting
        /// </summary>
        IDisposable Take<T>(IQuery<T> query, Action<T> callback);
        /// <summary>
        /// read all matching entities
        /// </summary>
        IReadOnlyList<T> Scan<T>(IQuery<T> query);
        /// <summary>
        /// count all matching entities
        /// </summary>
        int Count<T>(IQuery<T> query);
        /// <summary>
        /// add new entity
        /// </summary>
        void Add<T>(TupleType type, T tuple);
        /// <summary>
        /// add bunch of new entities
        /// </summary>
        void AddRange<T>(TupleType type, T[] tuples);
    }
}