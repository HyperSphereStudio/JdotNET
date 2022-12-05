using System;
using System.Collections;
using System.Collections.Generic;

namespace JULIAdotNET {
    public interface JVal<T> {
        protected internal T This { get; }
    }

    public interface JEnumerable<ObjectT, EnumerableT, StateT, IndexT> : 
            JVal<ObjectT>, IEnumerable<EnumerableT>
            
            where ObjectT : JEnumerable<ObjectT, EnumerableT, StateT, IndexT>{
        
        protected internal void EnumerationReset(StateT s, out StateT ns);
        protected internal EnumerableT EnumerationCurrent(StateT s);
        protected internal void EnumerationDispose();
        protected internal bool EnumerationMoveNext(StateT s, out StateT ns);
        protected internal IndexT EnumerationIndex(StateT s);
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<EnumerableT> IEnumerable<EnumerableT>.GetEnumerator() => new JEnumerator<ObjectT, EnumerableT, StateT, IndexT>(This);
    }
    
    public struct JEnumerator<ObjectT, EnumerableT, StateT, IndexT> : IEnumerator<EnumerableT> where ObjectT : JEnumerable<ObjectT, EnumerableT, StateT, IndexT> {
        private readonly ObjectT _ptr;
        private StateT _state;
        public IndexT Index => _ptr.EnumerationIndex(_state);

        public JEnumerator(ObjectT ptr) {
            _ptr = ptr;
            _state = default;
            Reset();
        }

        public bool MoveNext() => _ptr.EnumerationMoveNext(_state, out _state);
        public void Reset() => _ptr.EnumerationReset(_state, out _state);
        public EnumerableT Current => _ptr.EnumerationCurrent(_state);
        object IEnumerator.Current => Current;
        public void Dispose() => _ptr.EnumerationDispose();
    }
}