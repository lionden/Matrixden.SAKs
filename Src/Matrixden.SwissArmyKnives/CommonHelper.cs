namespace Matrixden.Utils
{
    using Matrixden.Utils.Logging;
    using System;
    using System.Linq;

    public class CommonHelper
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// 俩实体类间的深拷贝.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceModel"></param>
        /// <param name="targetModel"></param>
        /// <exception cref="System.ArgumentNullException">Throw out ArgumentNullException when either source or target model is null.</exception>
        public static void SyncModel<T>(T sourceModel, T targetModel)
        {
            SyncModelToModel<T, T>(sourceModel, targetModel);
        }

        /// <summary>
        /// 将SourceModel里面与TargetModel里面字段名一样的数据拷贝到TargetModel里
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceModel"></param>
        /// <param name="targetModel"></param>
        /// <exception cref="System.ArgumentNullException">Throw out ArgumentNullException when either source or target model is null.</exception>
        public static void SyncModelToModel<S, T>(S sourceModel, T targetModel)
        {
            if (sourceModel == null || targetModel == null)
            {
                throw new ArgumentNullException("Both source and target model cannot be NULL.");
            }

            var ps = typeof(T).GetProperties();
            if (ps == null || ps.Length <= 0)
                return;

            foreach (var t in ps.Where(w => w.CanWrite))
            {
                var s = typeof(S).GetProperty(t.Name);
                if (s != null)
                {
                    t.SetValue(targetModel, s.GetValue(sourceModel));
                }
            }
        }
    }
}
