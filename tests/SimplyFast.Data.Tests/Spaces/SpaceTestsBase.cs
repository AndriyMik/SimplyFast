﻿using System.Linq;
using Xunit;
using SimplyFast.Data.Spaces.Interface;

namespace SimplyFast.Data.Tests.Spaces
{
    public abstract class SpaceTestsBase
    {
        public abstract ISpace Space { get; }
        public abstract ISpaceProxy SpaceProxy { get; }

        private static readonly TupleType _testTupleType = new TupleType(0);

        private static readonly SpaceTuple _tuple00 = new SpaceTuple(0, 0);
        private static readonly SpaceTuple _tuple10 = new SpaceTuple(1, 0);
        private static readonly SpaceTuple _tuple11 = new SpaceTuple(1, 1);
        private static readonly SpaceTuple _tuple20 = new SpaceTuple(2, 0);


        private static readonly SpaceTupleQuery _tuple1Q = new SpaceTupleQuery(1, null);
        private static readonly SpaceTupleQuery _tuple2Q = new SpaceTupleQuery(2, null);
        private static readonly SpaceTupleQuery _tuplesq = new SpaceTupleQuery(null, null);

        [Fact]
        public void CanWrite()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            var result = SpaceProxy.TryRead(_tuple1Q);
            Assert.Equal(_tuple10, result);
        }

        [Fact]
        public void CanTryRead()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);
            var result = SpaceProxy.TryRead(_tuple1Q);
            Assert.Equal(_tuple10, result);
            result = SpaceProxy.TryRead(new SpaceTupleQuery(1, 0));
            Assert.Equal(_tuple10, result);
            result = SpaceProxy.TryRead(_tuple2Q);
            Assert.Equal(_tuple20, result);
            result = SpaceProxy.TryRead(new SpaceTupleQuery(3, null));
            Assert.Null(result);
        }

        [Fact]
        public void CanScan()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple11);
            SpaceProxy.Add(_testTupleType, _tuple20);
            var result = SpaceProxy.Scan(_tuple1Q);
            Assert.True(result.OrderBy(x => x.Y).SequenceEqual(new[] { _tuple10, _tuple11 }));
            result = SpaceProxy.Scan(_tuple2Q);
            Assert.True(result.SequenceEqual(new[] { _tuple20 }));
        }

        [Fact]
        public void CanTryTake()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);
            var result = SpaceProxy.TryTake(_tuple1Q);
            Assert.Equal(_tuple10, result);
            result = SpaceProxy.TryTake(_tuple1Q);
            Assert.Null(result);
            result = SpaceProxy.TryRead(_tuple1Q);
            Assert.Null(result);
            result = SpaceProxy.TryTake(_tuple2Q);
            Assert.Equal(_tuple20, result);
        }

        [Fact]
        public void CanCount()
        {
            SpaceProxy.AddRange(_testTupleType, new[] { _tuple10, _tuple11, _tuple20 });
            Assert.Equal(0, SpaceProxy.Count(new SpaceTupleQuery(2, 1)));
            Assert.Equal(1, SpaceProxy.Count(_tuple2Q));
            Assert.Equal(2, SpaceProxy.Count(_tuple1Q));
            Assert.Equal(3, SpaceProxy.Count(_tuplesq));
            var obj = SpaceProxy.TryTake(_tuple1Q);
            Assert.NotNull(obj);
            Assert.Equal(0, SpaceProxy.Count(new SpaceTupleQuery(2, 1)));
            Assert.Equal(1, SpaceProxy.Count(_tuple2Q));
            Assert.Equal(1, SpaceProxy.Count(_tuple1Q));
            Assert.Equal(2, SpaceProxy.Count(_tuplesq));
        }

        [Fact]
        public void CanRead()
        {
            SpaceProxy.AddRange(_testTupleType, new[] { _tuple10, _tuple20 });
            SpaceTuple result = null;
            // instant read
            SpaceProxy.Read(_tuple1Q, r => result = r);
            Assert.Equal(_tuple10, result);
            result = null;
            SpaceProxy.Read(_tuple1Q, r => result = r);
            Assert.Equal(_tuple10, result);

            result = SpaceProxy.TryTake(_tuple1Q);
            Assert.Equal(_tuple10, result);
            result = null;
            SpaceProxy.Read(_tuple1Q, r => result = r);
            Assert.Null(result);
            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Equal(_tuple10, result);
            SpaceProxy.Add(_testTupleType, _tuple11);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Equal(_tuple10, result);


            Assert.Equal(_tuple11, SpaceProxy.TryTake(new SpaceTupleQuery(1, 1)));
            Assert.Equal(_tuple10, SpaceProxy.TryTake(new SpaceTupleQuery(1, 0)));

            result = _tuple00;
            var cancel = SpaceProxy.Read(new SpaceTupleQuery(1, null), r => result = r);
            Assert.Equal(_tuple00, result);
            // cancel read
            cancel.Dispose();
            Assert.Null(result);

            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Null(result);
        }

        [Fact]
        public void CanTake()
        {
            SpaceProxy.AddRange(_testTupleType, new[] { _tuple10, _tuple20 });
            SpaceTuple result = null;

            // instant take
            SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.Equal(_tuple10, result);

            // wait take
            result = null;
            SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.Null(result);
            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Equal(_tuple10, result);
            // wait take only once
            SpaceProxy.Add(_testTupleType, _tuple11);


            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Equal(_tuple10, result);
            SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.Equal(_tuple11, result);

            result = _tuple00;
            var cancel = SpaceProxy.Take(_tuple1Q, r => result = r);
            Assert.Equal(_tuple00, result);
            // cancel take
            cancel.Dispose();
            Assert.Null(result);

            SpaceProxy.Add(_testTupleType, _tuple10);
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Null(result);

            // check that tuple was not taken
            Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));
        }

        private void CanAddInTransactionsAndNested(bool commit)
        {
            var cleanProxy = Space.CreateProxy();

            SpaceProxy.BeginTransaction();
            {
                // add tuple in transaction
                SpaceProxy.Add(_testTupleType, _tuple10);
                // space should not see it
                Assert.Null(cleanProxy.TryRead(_tuple1Q));
                // trans should see it
                Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));
                SpaceProxy.BeginTransaction();
                {
                    // nested should see it
                    Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));

                    // add tuple2 in nested transaction
                    SpaceProxy.Add(_testTupleType, _tuple20);

                    // space should not see it
                    Assert.Null(cleanProxy.TryRead(_tuple2Q));
                    // nested should see it
                    Assert.Equal(_tuple20, SpaceProxy.TryRead(_tuple2Q));

                    SpaceProxy.CommitTransaction();
                }
                // trans should see it now
                Assert.Equal(_tuple20, SpaceProxy.TryRead(_tuple2Q));
                // space still should not see it
                Assert.Null(cleanProxy.TryRead(_tuple2Q));

                if (commit)
                {
                    SpaceProxy.CommitTransaction();
                    // now space should see both
                    Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));
                    Assert.Equal(_tuple20, SpaceProxy.TryRead(_tuple2Q));
                    Assert.Equal(_tuple10, cleanProxy.TryRead(_tuple1Q));
                    Assert.Equal(_tuple20, cleanProxy.TryRead(_tuple2Q));

                }
                else
                {
                    SpaceProxy.RollbackTransaction();
                    // space should see nothing
                    Assert.Null(SpaceProxy.TryRead(_tuple1Q));
                    Assert.Null(SpaceProxy.TryRead(_tuple2Q));
                    Assert.Null(cleanProxy.TryRead(_tuple1Q));
                    Assert.Null(cleanProxy.TryRead(_tuple2Q));
                }

            }
        }

        [Fact]
        public void CanAddInTransactionsAndNested()
        {
            CanAddInTransactionsAndNested(false);
            CanAddInTransactionsAndNested(true);
        }


        private void CanAddInNestedTransactions(bool commitNested, bool commitTrans)
        {
            var cleanProxy = Space.CreateProxy();

            SpaceProxy.BeginTransaction();
            {
                SpaceTuple trans10Result = null;
                SpaceProxy.Read(_tuple1Q, x => trans10Result = x);

                SpaceTuple trans20Result = null;
                SpaceProxy.Read(_tuple2Q, x => trans20Result = x);
                SpaceProxy.BeginTransaction();
                {
                    SpaceProxy.Add(_testTupleType, _tuple10);
                    // space should not see it
                    Assert.Null(cleanProxy.TryRead(_tuple1Q));
                    // trans should not see it
                    Assert.Null(trans10Result);
                    // nested should see it
                    Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));

                    SpaceTuple nestedResult = null;
                    SpaceProxy.Read(_tuple2Q, x => nestedResult = x);
                    SpaceProxy.BeginTransaction();
                    {
                        // nested2 should see it
                        Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));

                        // add in nested 2
                        SpaceProxy.Add(_testTupleType, _tuple20);

                        // space should not see it
                        Assert.Null(cleanProxy.TryRead(_tuple2Q));
                        // trans should not see it
                        Assert.Null(trans20Result);
                        // nested should not see it
                        Assert.Null(nestedResult);
                        // nested2 should see it
                        Assert.Equal(_tuple20, SpaceProxy.TryRead(_tuple2Q));

                        SpaceProxy.CommitTransaction();
                    }
                    // space should not see it
                    Assert.Null(cleanProxy.TryRead(_tuple2Q));
                    // trans should not see it
                    Assert.Null(trans20Result);
                    // nested should see it
                    // ReSharper disable once ExpressionIsAlwaysNull
                    Assert.Equal(_tuple20, nestedResult);
                    if (commitNested)
                    {
                        SpaceProxy.CommitTransaction();
                    }
                    else
                    {
                        SpaceProxy.RollbackTransaction();
                    }
                }
                // space should not see both
                Assert.Null(cleanProxy.TryRead(_tuple1Q));
                Assert.Null(cleanProxy.TryRead(_tuple2Q));
                if (commitNested)
                {
                    // trans should see both
                    Assert.Equal(_tuple10, trans10Result);
                    Assert.Equal(_tuple20, trans20Result);
                }
                else
                {
                    // trans should not see both
                    Assert.Null(trans10Result);
                    Assert.Null(trans20Result);
                }

                if (commitTrans)
                {
                    SpaceProxy.CommitTransaction();
                }
                else
                {
                    SpaceProxy.RollbackTransaction();
                }

                if (commitTrans && commitNested)
                {
                    // now space should see both
                    Assert.Equal(_tuple10, cleanProxy.TryRead(_tuple1Q));
                    Assert.Equal(_tuple20, cleanProxy.TryRead(_tuple2Q));
                }
                else
                {
                    // space still should not see both
                    Assert.Null(cleanProxy.TryRead(_tuple1Q));
                    Assert.Null(cleanProxy.TryRead(_tuple2Q));
                }
            }
        }

        [Fact]
        public void CanAddInNestedTransactions()
        {
            CanAddInNestedTransactions(false, false);
            CanAddInNestedTransactions(false, true);
            CanAddInNestedTransactions(true, false);
            CanAddInNestedTransactions(true, true);
        }



        [Fact]
        public void DisposeAbortsTransaction()
        {
            using (var proxy = Space.CreateProxy())
            {
                proxy.BeginTransaction();
                proxy.Add(_testTupleType, _tuple10);
                // verify add
                Assert.Equal(_tuple10, proxy.TryRead(_tuple1Q));

                proxy.BeginTransaction();
                proxy.Add(_testTupleType, _tuple20);
                // verify add
                Assert.Equal(_tuple10, proxy.TryRead(_tuple1Q));
                Assert.Equal(_tuple20, proxy.TryRead(_tuple2Q));
            }
            // nothing in the space
            Assert.Null(SpaceProxy.TryRead(_tuple1Q));
            Assert.Null(SpaceProxy.TryRead(_tuple2Q));
        }

        [Fact]
        public void DisposeAbortsWaitingActions()
        {
            var globalResult = _tuple00;
            var transactionResult = _tuple00;
            using (var proxy = Space.CreateProxy())
            {
                proxy.Read(_tuple1Q, x => globalResult = x);
                proxy.BeginTransaction();

                proxy.Add(_testTupleType, _tuple10);
                Assert.Equal(_tuple00, globalResult);
                Assert.Equal(_tuple00, transactionResult);

                proxy.Read(_tuple2Q, x => transactionResult = x);
                Assert.Equal(_tuple00, globalResult);
                Assert.Equal(_tuple00, transactionResult);
            }
            Assert.Null(globalResult);
            Assert.Null(transactionResult);
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);
            Assert.Null(globalResult);
            Assert.Null(transactionResult);
        }

        [Fact]
        public void AbortAndCommitAbortsWaitingAction()
        {
            var tresult = _tuple00;

            SpaceProxy.BeginTransaction();
            SpaceProxy.Read(_tuple1Q, x => tresult = x);
            Assert.Equal(_tuple00, tresult);
            SpaceProxy.RollbackTransaction();
            Assert.Null(tresult);
            // check that wa was deleted
            SpaceProxy.Add(_testTupleType, _tuple10);
            Assert.Null(tresult);

            tresult = _tuple00;
            SpaceProxy.BeginTransaction();
            SpaceProxy.Read(_tuple2Q, x => tresult = x);
            Assert.Equal(_tuple00, tresult);
            SpaceProxy.RollbackTransaction();
            Assert.Null(tresult);
            // check that wa was deleted
            SpaceProxy.Add(_testTupleType, _tuple20);
            Assert.Null(tresult);
        }

        [Fact]
        public void OtherProxyTriggersGlobalWait()
        {
            var tresult = _tuple00;

            SpaceProxy.Read(_tuple1Q, x => tresult = x);
            Assert.Equal(_tuple00, tresult);
            using (var proxy = Space.CreateProxy())
            {
                proxy.Add(_testTupleType, _tuple10);
                Assert.Equal(_tuple10, tresult);
                Assert.Equal(1, proxy.Count(_tuple1Q));
            }

            SpaceProxy.Take(_tuple2Q, x => tresult = x);
            Assert.Equal(_tuple10, tresult);
            using (var proxy = Space.CreateProxy())
            {
                proxy.Add(_testTupleType, _tuple20);
                Assert.Equal(_tuple20, tresult);
                Assert.Equal(0, proxy.Count(_tuple2Q));
            }
        }

        [Fact]
        public void TransactionCommitTriggersWait()
        {
            var gresult = _tuple00;
            var tresult = _tuple00;

            SpaceProxy.Read(_tuple1Q, x => gresult = x);

            SpaceProxy.BeginTransaction();
            SpaceProxy.Read(_tuple1Q, x => tresult = x);

            SpaceProxy.BeginTransaction();
            Assert.Equal(_tuple00, gresult);
            Assert.Equal(_tuple00, tresult);

            SpaceProxy.Add(_testTupleType, _tuple10);

            // nothing should be triggered for uncommited trans
            Assert.Equal(_tuple00, gresult);
            Assert.Equal(_tuple00, tresult);
            SpaceProxy.CommitTransaction();
            // now transactional read should be triggered
            Assert.Equal(_tuple00, gresult);
            Assert.Equal(_tuple10, tresult);
            SpaceProxy.CommitTransaction();
            // now boths reads should be triggered
            Assert.Equal(_tuple10, gresult);
            Assert.Equal(_tuple10, tresult);
        }

        [Fact]
        public void GlobalsTryTakenInTransactionAreFine()
        {
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.Add(_testTupleType, _tuple20);

            SpaceProxy.BeginTransaction();
            Assert.Equal(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            SpaceProxy.CommitTransaction();
            Assert.Null(SpaceProxy.TryTake(_tuple1Q));

            SpaceProxy.BeginTransaction();
            Assert.Equal(_tuple20, SpaceProxy.TryTake(_tuple2Q));
            SpaceProxy.RollbackTransaction();
            Assert.Equal(_tuple20, SpaceProxy.TryTake(_tuple2Q));
            Assert.Null(SpaceProxy.TryTake(_tuple2Q));
        }

        [Fact]
        public void GlobalsTakenInTransactionAreFine()
        {
            using (var proxy = Space.CreateProxy())
            {
                var tresult = _tuple00;

                SpaceProxy.BeginTransaction();

                SpaceProxy.Take(_tuple1Q, x => tresult = x);
                proxy.Add(_testTupleType, _tuple10);

                Assert.Equal(_tuple10, tresult);
                Assert.Null(SpaceProxy.TryTake(_tuple1Q));
                Assert.Null(proxy.TryTake(_tuple1Q));

                SpaceProxy.CommitTransaction();
                Assert.Null(SpaceProxy.TryTake(_tuple1Q));
                Assert.Null(proxy.TryTake(_tuple1Q));



                SpaceProxy.BeginTransaction();

                SpaceProxy.Take(_tuple2Q, x => tresult = x);
                proxy.Add(_testTupleType, _tuple20);
                Assert.Equal(_tuple20, tresult);

                Assert.Null(SpaceProxy.TryTake(_tuple2Q));
                Assert.Null(proxy.TryTake(_tuple2Q));

                SpaceProxy.RollbackTransaction();

                Assert.Equal(_tuple20, SpaceProxy.TryTake(_tuple2Q));
                Assert.Null(SpaceProxy.TryTake(_tuple2Q));
            }
        }

        [Fact]
        public void GlobalTakesAndReadsTransactionCommits()
        {
            using (var proxy = Space.CreateProxy())
            {
                var tresult = _tuple00;
                var result = _tuple00;
                proxy.Read(_tuple1Q, x => result = x);
                proxy.Take(_tuple2Q, x => tresult = x);

                SpaceProxy.BeginTransaction();
                SpaceProxy.Add(_testTupleType, _tuple10);
                SpaceProxy.Add(_testTupleType, _tuple20);
                Assert.Equal(_tuple00, tresult);
                Assert.Equal(_tuple00, result);
                SpaceProxy.CommitTransaction();

                Assert.Equal(_tuple10, result);
                Assert.Equal(_tuple20, tresult);
            }
        }

        [Fact]
        public void GlobalTakesAndReadsTransactionCommitsEvenWhileTransWasRunning()
        {
            using (var proxy = Space.CreateProxy())
            {
                var tresult = _tuple00;
                var result = _tuple00;


                SpaceProxy.BeginTransaction();
                SpaceProxy.Add(_testTupleType, _tuple10);
                SpaceProxy.Add(_testTupleType, _tuple20);

                proxy.Read(_tuple1Q, x => result = x);
                proxy.Take(_tuple2Q, x => tresult = x);
                Assert.Equal(_tuple00, tresult);
                Assert.Equal(_tuple00, result);
                SpaceProxy.CommitTransaction();

                Assert.Equal(_tuple10, result);
                Assert.Equal(_tuple20, tresult);
            }
        }

        [Fact]
        public void AllReadersReadTuple()
        {
            var results = Enumerable.Range(0, 10).Select(x => _tuple00).ToArray();
            for (var i = 0; i < results.Length; i++)
            {
                var i1 = i;
                SpaceProxy.Read(_tuple1Q, x => results[i1] = x);
            }
            Assert.True(results.All(x => x.Equals(_tuple00)));
            SpaceProxy.Add(_testTupleType, _tuple10);
            Assert.True(results.All(x => x.Equals(_tuple10)));
        }

        [Fact]
        public void TransactionTakesByTransactionAreFine()
        {
            SpaceProxy.BeginTransaction();
            SpaceProxy.Add(_testTupleType, _tuple10);
            SpaceProxy.BeginTransaction();
            Assert.Equal(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            Assert.Null(SpaceProxy.TryTake(_tuple1Q));
            SpaceProxy.RollbackTransaction();
            Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));

            SpaceProxy.BeginTransaction();
            Assert.Equal(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            Assert.Null(SpaceProxy.TryTake(_tuple1Q));
            SpaceProxy.CommitTransaction();
            Assert.Null(SpaceProxy.TryRead(_tuple1Q));
            SpaceProxy.CommitTransaction();
            Assert.Null(SpaceProxy.TryRead(_tuple1Q));
        }

        [Fact]
        public void TransactionTakesByMoreNestedTransactionAreFine()
        {
            SpaceProxy.BeginTransaction();
            SpaceProxy.Add(_testTupleType, _tuple10);

            SpaceProxy.BeginTransaction();

            SpaceProxy.BeginTransaction();

            Assert.Equal(_tuple10, SpaceProxy.TryTake(_tuple1Q));
            Assert.Null(SpaceProxy.TryTake(_tuple1Q));

            SpaceProxy.RollbackTransaction();

            Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));

            SpaceProxy.CommitTransaction();

            Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));

            SpaceProxy.CommitTransaction();

            Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));
        }


        [Fact]
        public void TransTakesAndReadsNestedTransactionCommits()
        {
            var tresult = _tuple00;
            var result = _tuple00;

            SpaceProxy.BeginTransaction();

            SpaceProxy.Read(_tuple1Q, x => result = x);
            SpaceProxy.Take(_tuple2Q, x => tresult = x);

            SpaceProxy.BeginTransaction();

            SpaceProxy.Add(_testTupleType, _tuple10);
            Assert.Equal(_tuple00, tresult);
            Assert.Equal(_tuple00, result);


            SpaceProxy.BeginTransaction();
            SpaceProxy.Add(_testTupleType, _tuple20);
            Assert.Equal(_tuple00, tresult);
            Assert.Equal(_tuple00, result);
            SpaceProxy.CommitTransaction();
            Assert.Equal(_tuple00, tresult);
            Assert.Equal(_tuple00, result);

            SpaceProxy.CommitTransaction();
            Assert.Equal(_tuple10, result);
            Assert.Equal(_tuple20, tresult);
            SpaceProxy.CommitTransaction();

            Assert.Equal(_tuple10, result);
            Assert.Equal(_tuple20, tresult);

            Assert.Equal(_tuple10, SpaceProxy.TryRead(_tuple1Q));
            Assert.Null(SpaceProxy.TryRead(_tuple2Q));
        }

        private void ScanCountInTransaction(int current, int count)
        {
            if (current >= count)
                return;

            SpaceProxy.BeginTransaction();

            Assert.Equal(current, SpaceProxy.Count(_tuple1Q));
            var scan = SpaceProxy.Scan(_tuple1Q);
            Assert.Equal(current, scan.Count);
            Assert.True(scan.All(_tuple10.Equals));

            // after adding, all stuff should be  +1
            SpaceProxy.Add(_testTupleType, _tuple10);
            current++;

            Assert.Equal(current, SpaceProxy.Count(_tuple1Q));
            scan = SpaceProxy.Scan(_tuple1Q);
            Assert.Equal(current, scan.Count);
            Assert.True(scan.All(_tuple10.Equals));

            ScanCountInTransaction(current, count);

            // now we should have count stuff

            Assert.Equal(count, SpaceProxy.Count(_tuple1Q));
            scan = SpaceProxy.Scan(_tuple1Q);
            Assert.Equal(count, scan.Count);
            Assert.True(scan.All(_tuple10.Equals));

            SpaceProxy.CommitTransaction();
        }

        [Fact]
        public void ScanAndCountTakeTransItems()
        {
            ScanCountInTransaction(0, 10);
        }
    }
}