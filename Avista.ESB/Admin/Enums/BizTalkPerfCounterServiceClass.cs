using System;

namespace Avista.ESB.Admin.Enums
{
      public sealed class BizTalkPerfCounterServiceClass
      {
            public static BizTalkPerfCounterServiceClass XLANGs = new BizTalkPerfCounterServiceClass( new System.Guid( "226fc6b9-0416-47a4-a8e8-4721f1db1a1b" ) );
            public static BizTalkPerfCounterServiceClass MessagingInProcess = new BizTalkPerfCounterServiceClass( new System.Guid( "59f295b0-3123-416e-966b-a2c6d65ff8e6" ) );
            public static BizTalkPerfCounterServiceClass MessagingIsolated = new BizTalkPerfCounterServiceClass( new System.Guid( "683aedf1-027d-4006-ae9a-443d1a5746fc" ) );
            public static BizTalkPerfCounterServiceClass MSMQT = new BizTalkPerfCounterServiceClass( new System.Guid( "3d7a3f58-4bfb-4593-b99e-c2a5dc35a3b2" ) );

            public Guid ClassID;

            public BizTalkPerfCounterServiceClass (Guid classID)
            {
                  ClassID = classID;
            }

            public static explicit operator BizTalkPerfCounterServiceClass (Guid classID)
            {
                  if ( classID.CompareTo( XLANGs ) == 0 )
                        return new BizTalkPerfCounterServiceClass( XLANGs );

                  else if ( classID.CompareTo( MessagingInProcess ) == 0 )
                        return new BizTalkPerfCounterServiceClass( MessagingInProcess );

                  else if ( classID.CompareTo( MessagingIsolated ) == 0 )
                        return new BizTalkPerfCounterServiceClass( MessagingInProcess );

                  else if ( classID.CompareTo( MSMQT ) == 0 )
                        return new BizTalkPerfCounterServiceClass( MSMQT );

                  throw new ArgumentException();
            }

            public static implicit operator Guid (BizTalkPerfCounterServiceClass serviceClass)
            {
                  return serviceClass.ClassID;
            }
      }
}