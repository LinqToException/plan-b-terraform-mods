﻿// Copyright (c) David Karnok, 2023
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FeatMultiplayer
{
    internal class MessageUpdateStacksAndContentDataAt : MessageBase
    {
        const string messageCode = "UpdateStacksAndContentDataAt";
        static readonly byte[] messageCodeBytes = Encoding.UTF8.GetBytes(messageCode);
        public override string MessageCode() => messageCode;
        public override byte[] MessageCodeBytes() => messageCodeBytes;

        internal int2 coords;
        internal bool updateBlocks;
        internal uint contentData;
        internal readonly List<SnapshotStack> stacks = new();

        public void GetSnapshot(int2 coords, bool updateBlocks)
        {
            this.coords = coords;
            this.updateBlocks = updateBlocks;
            contentData = GHexes.contentData[coords.x, coords.y];
            var gstacks = GHexes.stacks[coords.x, coords.y];

            if (gstacks != null)
            {
                for (int i = 0; i < gstacks.stacks.Length; i++)
                {
                    var ssnp = new SnapshotStack();
                    ssnp.GetSnapshot(in gstacks.stacks[i]);
                    stacks.Add(ssnp);
                }
            }
        }

        public void ApplySnapshot()
        {
            GHexes.contentData[coords.x, coords.y] = contentData;
            var lookup = Plugin.GetItemsDictionary();
            var gstacks = GHexes.stacks[coords.x, coords.y];
            if (gstacks != null)
            {
                for (int i = 0; i < stacks.Count; i++)
                {
                    if (i < gstacks.stacks.Length)
                    {
                        var ssnp = stacks[i];
                        ssnp.ApplySnapshot(ref gstacks.stacks[i], lookup);
                    }
                    else
                    {
                        gstacks.stacks[i].Reset();
                    }
                }
            }
        }

        public override void Encode(BinaryWriter output)
        {
            output.Write(coords.x);
            output.Write(coords.y);
            output.Write(updateBlocks);
            output.Write(contentData);
            output.Write(stacks.Count);
            foreach (var s in stacks)
            {
                s.Encode(output);
            }
        }

        public override bool TryDecode(BinaryReader input, out MessageBase message)
        {
            var msg = new MessageUpdateStacksAndContentDataAt();
            msg.Decode(input);

            message = msg;
            return true;
        }

        void Decode(BinaryReader input)
        {
            coords = new int2(input.ReadInt32(), input.ReadInt32());
            updateBlocks = input.ReadBoolean();
            contentData = input.ReadUInt32();
            int c = input.ReadInt32();
            for (int i = 0; i < c; i++)
            {
                var s = new SnapshotStack();
                s.Decode(input);
                stacks.Add(s);
            }
        }
    }
}