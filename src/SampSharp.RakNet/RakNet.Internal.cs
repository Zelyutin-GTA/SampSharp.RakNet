// SampSharp.RakNet
// Copyright 2018 Danil Zelyutin
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using SampSharp.Core.Natives.NativeObjects;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        protected static RakNetInternal Internal { get; set; }

        static RakNet()
        {
            Internal = NativeObjectProxyFactory.CreateInstance<RakNetInternal>();
        }

        public class RakNetInternal
        {
            [NativeMethod]
            public virtual int CallRemoteFunction(string functionName, string argumentsFormat)
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int PR_RegHandler(int name, string functionName, int type)
            {
                throw new NativeNotImplementedException();
            }
        }
    }
}
