using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.EFCore.LibQuery
{
    public class ClaimQueryLibrary
    {
        public partial class ClaimDealerSql
        {
            public static readonly string MaxSeqnumWithWhere = @"SELECT (isnull(MAX(CAST(SeqNo  AS Int)),0)+1) Value FROM |TABLE| |Where| ;";
            public static readonly string CountMasterPart = @"Select Count(1) Value FROM MasterPart Where PartNo = @PartNo AND IsDeleted=0;";
            public static readonly string CountTableWithWhere = @"Select Count(1) Value FROM |Table| |Where| ;";
            public static readonly string PricePartSql = $@"
                                                            DECLARE @Spr DECIMAL(18,4)
                                                            DECLARE @Fob DECIMAL(18,4)
                                                            DECLARE @CdImport VARCHAR(50)
                                                            DECLARE @Frenc VARCHAR(50)
                                                            SELECT @Spr = CAST(Spr as Decimal(18,4)), @Fob = CAST(Fob as Decimal(18,4)), @CdImport = CdImport, @Frenc = Frenc
                                                            FROM MasterPart
                                                            WHERE Partno = @Partno
                                                            SELECT isnull(@Spr,0) as Spr, isnull(@Fob, 0) as Fob, isnull(@CdImport) as CdImport, @Frenc as Frenc;
                                                            ";
            public static readonly string AmountPart = @"SELECT systemvalue Value FROM mastersystemconfig  WHERE IsDeleted=0 AND  systemcategory='TAM' AND Systemsubcategory='PRR' AND systemcode= (SELECT top 1 cdimport FROM masterpart WHERE  IsDeleted=0 AND  partno = @PartNo);";
        }
    }
}
