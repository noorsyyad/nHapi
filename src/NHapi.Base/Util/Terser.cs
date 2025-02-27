/*
  The contents of this file are subject to the Mozilla Public License Version 1.1
  (the "License"); you may not use this file except in compliance with the License.
  You may obtain a copy of the License at http://www.mozilla.org/MPL/
  Software distributed under the License is distributed on an "AS IS" basis,
  WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for the
  specific language governing rights and limitations under the License.

  The Original Code is "Terser.java".  Description:
  "Wraps a message to provide access to fields using a more terse syntax."

  The Initial Developer of the Original Code is University Health Network. Copyright (C)
  2002.  All Rights Reserved.

  Contributor(s): ______________________________________.

  Alternatively, the contents of this file may be used under the terms of the
  GNU General Public License (the "GPL"), in which case the provisions of the GPL are
  applicable instead of those above.  If you wish to allow use of your version of this
  file only under the terms of the GPL and not to allow others to use your version
  of this file under the MPL, indicate your decision by deleting  the provisions above
  and replace  them with the notice and other provisions required by the GPL License.
  If you do not delete the provisions above, a recipient may use your version of
  this file under either the MPL or the GPL.
*/

// TODO: Review examples in XML doc comments.
// most of the code examples are for hapi, these needs reviewing as part of documentation ticket.
namespace NHapi.Base.Util
{
    using System;

    using NHapi.Base.Log;
    using NHapi.Base.Model;

    /// <summary>
    /// Wraps a message to provide access to fields using a terse location specification syntax.
    /// </summary>
    /// <example>
    /// <para>
    /// For example:
    /// <code>terser.set("MSH-9-3", "ADT_A01");</code> <br />
    /// can be used instead of. <br />
    /// <code>message.getMSH().getMessageType().getMessageStructure().setValue("ADT_A01"); </code>
    /// </para>
    /// <para>
    /// The syntax of a location spec is as follows: <br />
    /// location_spec: <code>segment_path_spec "-" field ["(" rep ")"] ["-" component ["-" subcomponent]] </code> <br />
    /// ... where rep, field, component, and subcomponent are integers (representing, respectively,
    /// the field repetition (starting at 0), and the field number, component number, and subcomponent
    /// numbers (starting at 1).  Omitting the rep is equivalent to specifying 0; omitting the
    /// component or subcomponent is equivalent to specifying 1.
    /// </para>
    /// <para>
    /// The syntax for the segment_path_spec is as follows: <br />
    /// segment_path_spec: <code> ["/"] (group_spec ["(" rep ")"] "/")* segment_spec ["(" rep ")"]</code><br />
    /// ... where rep has the same meaning as for fields.  A leading "/" indicates that navigation to the
    /// location begins at the root of the message; omitting this indicates that navigation begins at the
    /// current location of the underlying SegmentFinder (see getFinder() -- this allows manual navigation
    /// if desired).
    /// </para>
    /// <para>
    /// The syntax for group_spec is: <br />
    /// group_spec: <code>["."] group_name_pattern</code><br />
    /// Here, a . indicates that the group should be searched for (using a SegmentFinder) starting at the
    /// current location in the message.  The wildcards "*" and "?" represent any number of arbitrary characters,
    /// and a single arbitrary character, respectively.  For example, "M*" and "?S?" match MSH.  The first
    /// group with a name that matches the given group_name_pattern will be matched.
    /// </para>
    /// <para>
    /// The segment_spec is analogous to the group_spec. <br />
    /// As another example, the following subcomponent in an SIU_S12 message: <br />
    /// <code>msg.getSIU_S12_RGSAISNTEAIGNTEAILNTEAIPNTE(1).getSIU_S12_AIGNTE().getAIG().getResourceGroup(1).getIdentifier();</code><br />
    /// ... is referenced by all of the following location_spec: <br />
    /// <code>
    /// /SIU_S12_RGSAISNTEAIGNTEAILNTEAIPNTE(1)/SIU_S12_AIGNTE/AIG-5(1)-1 <br/>
    /// /*AIG*(1)/SIU_S12_AIGNTE/AIG-5(1)-1 <br/>
    /// /*AIG*(1)/.AIG-5(1)
    /// </code>
    /// </para>
    /// <para>
    /// The search function only iterates through rep 0 of each group. Thus if rep 0 of the first group
    /// in this example was desired instead of rep 1, the following syntax would also work (since there is
    /// only one AIG segment position in SUI_S12): <code>/.AIG-5(1)</code>
    /// </para>
    /// </example>
    /// <author>Bryan Tripp.</author>
    public class Terser
    {
        private static IHapiLog log;

        static Terser()
        {
            log = HapiLogFactory.GetHapiLog(typeof(Terser));
        }

        /// <summary>Creates a new instance of Terser. </summary>
        public Terser(IMessage message)
        {
            Finder = new SegmentFinder(message);
        }

        /// <summary>
        /// Returns the segment finder used by this Terser.  Navigating the
        /// finder will influence the behaviour of the Terser accordingly. ie:
        /// when the full path of the segment is not specified the segment will
        /// be sought beginning at the current location of the finder.
        /// </summary>
        public virtual SegmentFinder Finder { get; private set; }

        /// <summary> Returns the string value of the Primitive at the given location.</summary>
        /// <param name="segment">the segment from which to get the primitive.
        /// </param>
        /// <param name="field">the field number.
        /// </param>
        /// <param name="rep">the field repetition.
        /// </param>
        /// <param name="component">the component number (use 1 for primitive field).
        /// </param>
        /// <param name="subcomponent">the subcomponent number (use 1 for primitive component).
        /// </param>
        public static string Get(ISegment segment, int field, int rep, int component, int subcomponent)
        {
            var prim = GetPrimitive(segment, field, rep, component, subcomponent);
            return prim.Value;
        }

        /// <summary> Sets the string value of the Primitive at the given location.</summary>
        public static void Set(ISegment segment, int field, int rep, int component, int subcomponent, string value_Renamed)
        {
            var prim = GetPrimitive(segment, field, rep, component, subcomponent);
            prim.Value = value_Renamed;
        }

        [Obsolete("This method has been replaced by 'GetPrimitive'.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:Element should begin with upper-case letter",
            Justification = "As this is a public member, we will duplicate the method and mark this one as obsolete.")]
        public static IPrimitive getPrimitive(IType type, int component, int subcomponent)
        {
            return GetPrimitive(type, component, subcomponent);
        }

        /// <summary> Returns the Primitive object at the given location in the given field.
        /// It is intended that the given type be at the field level, although extra components
        /// will be added blindly if, for example, you provide a primitive subcomponent instead
        /// and specify component or subcomponent > 1.
        /// </summary>
        public static IPrimitive GetPrimitive(IType type, int component, int subcomponent)
        {
            var comp = GetComponent(type, component);
            var sub = GetComponent(comp, subcomponent);
            return GetPrimitive(sub);
        }

        [Obsolete("This method has been replaced by 'GetIndices'.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:Element should begin with upper-case letter",
            Justification = "As this is a public member, we will duplicate the method and mark this one as obsolete.")]
        public static int[] getIndices(string spec)
        {
            return GetIndices(spec);
        }

        /// <summary>
        /// Given a Terser path, returns an array containing field num, field rep,
        /// component, and subcomponent.
        /// </summary>
        public static int[] GetIndices(string spec)
        {
            var tok = new SupportClass.Tokenizer(spec, "-", false);
            tok.NextToken(); // skip over segment
            if (!tok.HasMoreTokens())
            {
                throw new HL7Exception("Must specify field in spec " + spec, ErrorCode.APPLICATION_INTERNAL_ERROR);
            }

            int[] ret = null;
            try
            {
                var fieldSpec = new SupportClass.Tokenizer(tok.NextToken(), "()", false);
                var fieldNum = int.Parse(fieldSpec.NextToken());
                var fieldRep = 0;
                if (fieldSpec.HasMoreTokens())
                {
                    fieldRep = int.Parse(fieldSpec.NextToken());
                }

                var component = 1;
                if (tok.HasMoreTokens())
                {
                    component = int.Parse(tok.NextToken());
                }

                var subcomponent = 1;
                if (tok.HasMoreTokens())
                {
                    subcomponent = int.Parse(tok.NextToken());
                }

                var result = new int[] { fieldNum, fieldRep, component, subcomponent };
                ret = result;
            }
            catch (FormatException)
            {
                throw new HL7Exception("Invalid integer in spec " + spec, ErrorCode.APPLICATION_INTERNAL_ERROR);
            }

            return ret;
        }

        [Obsolete("This method has been replaced by 'NumSubComponents'.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:Element should begin with upper-case letter",
            Justification = "As this is a public member, we will duplicate the method and mark this one as obsolete.")]
        public static int numSubComponents(IType type, int component)
        {
            return NumSubComponents(type, component);
        }

        /// <summary>
        /// Returns the number of sub-components in the specified component, ie:
        /// the number of standard sub-components (e.g. 6 for CE) plus any extra components that
        /// that have been added at runtime.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="component">numbered from 1.</param>
        public static int NumSubComponents(IType type, int component)
        {
            var n = -1;
            if (component == 1 && typeof(IPrimitive).IsAssignableFrom(type.GetType()))
            {
                // note that getComponent(primitive, 1) below returns the primitive
                // itself -- if we do numComponents on it, we'll end up with the
                // number of components in the field, not the number of subcomponents
                n = 1;
            }
            else
            {
                var comp = GetComponent(type, component);
                n = NumComponents(comp);
            }

            return n;
        }

        [Obsolete("This method has been replaced by 'NumComponents'.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:Element should begin with upper-case letter",
            Justification = "As this is a public member, we will duplicate the method and mark this one as obsolete.")]
        public static int numComponents(IType type)
        {
            return NumComponents(type);
        }

        /// <summary> Returns the number of components in the given type, i.e. the
        /// number of standard components (e.g. 6 for CE) plus any extra components that
        /// have been added at runtime.
        /// </summary>
        public static int NumComponents(IType type)
        {
            if (typeof(Varies).IsAssignableFrom(type.GetType()))
            {
                return NumComponents(((Varies)type).Data);
            }
            else
            {
                return NumStandardComponents(type) + type.ExtraComponents.NumComponents();
            }
        }

        /// <summary>
        /// Gets the string value of the field specified. See the class docs for syntax
        /// of the location spec.
        /// <para>
        /// If a repetition is omitted for a repeating segment or field, the first rep is used.
        /// If the component or subcomponent is not specified for a composite field, the first
        /// component is used (this allows one to write code that will work with later versions of
        /// the HL7 standard).
        /// </para>
        /// </summary>
        public virtual string Get(string spec)
        {
            var tok = new SupportClass.Tokenizer(spec, "-", false);
            var segment = GetSegment(tok.NextToken());

            var ind = GetIndices(spec);
            return Get(segment, ind[0], ind[1], ind[2], ind[3]);
        }

        [Obsolete("This method has been replaced by 'GetSegment'.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:Element should begin with upper-case letter",
            Justification = "As this is a public member, we will duplicate the method and mark this one as obsolete.")]
        public virtual ISegment getSegment(string segSpec)
        {
            return GetSegment(segSpec);
        }

        /// <summary> Returns the segment specified in the given segment_path_spec. </summary>
        public virtual ISegment GetSegment(string segSpec)
        {
            ISegment seg = null;

            if (segSpec.Substring(0, 1 - 0).Equals("/"))
            {
                Finder.Reset();
            }

            var tok = new SupportClass.Tokenizer(segSpec, "/", false);
            var finder = Finder;
            while (tok.HasMoreTokens())
            {
                var pathSpec = tok.NextToken();
                var ps = ParsePathSpec(pathSpec);
                if (tok.HasMoreTokens())
                {
                    ps.IsGroup = true;
                }
                else
                {
                    ps.IsGroup = false;
                }

                if (ps.IsGroup)
                {
                    IGroup g = null;
                    if (ps.Find)
                    {
                        g = finder.FindGroup(ps.Pattern, ps.Rep);
                    }
                    else
                    {
                        g = finder.GetGroup(ps.Pattern, ps.Rep);
                    }

                    finder = new SegmentFinder(g);
                }
                else
                {
                    if (ps.Find)
                    {
                        seg = finder.FindSegment(ps.Pattern, ps.Rep);
                    }
                    else
                    {
                        seg = finder.GetSegment(ps.Pattern, ps.Rep);
                    }
                }
            }

            return seg;
        }

        /// <summary> Sets the string value of the field specified.  See class docs for location spec syntax.</summary>
        public virtual void Set(string spec, string value_Renamed)
        {
            var tok = new SupportClass.Tokenizer(spec, "-", false);
            var segment = GetSegment(tok.NextToken());

            var ind = GetIndices(spec);
            if (log.DebugEnabled)
            {
                log.Debug("Setting " + spec + " seg: " + segment.GetStructureName() + " ind: " + ind[0] + " " + ind[1] + " " +
                             ind[2] + " " + ind[3]);
            }

            Set(segment, ind[0], ind[1], ind[2], ind[3], value_Renamed);
        }

        /// <summary> Returns the Primitive object at the given location.</summary>
        private static IPrimitive GetPrimitive(ISegment segment, int field, int rep, int component, int subcomponent)
        {
            var type = segment.GetField(field, rep);
            return GetPrimitive(type, component, subcomponent);
        }

        /// <summary> Attempts to extract a Primitive from the given type. If it's a composite,
        /// drills down through first components until a primitive is reached.
        /// </summary>
        private static IPrimitive GetPrimitive(IType type)
        {
            IPrimitive p = null;
            if (typeof(Varies).IsAssignableFrom(type.GetType()))
            {
                p = GetPrimitive(((Varies)type).Data);
            }
            else if (typeof(IComposite).IsAssignableFrom(type.GetType()))
            {
                try
                {
                    p = GetPrimitive(((IComposite)type)[0]);
                }
                catch (HL7Exception)
                {
                    throw new ApplicationException("Internal error: HL7Exception thrown on Composite.getComponent(0).");
                }
            }
            else if (type is IPrimitive)
            {
                p = (IPrimitive)type;
            }

            return p;
        }

        /// <summary> Returns the component (or sub-component, as the case may be) at the given
        /// index.  If it does not exist, it is added as an "extra component".
        /// If comp > 1 is requested from a Varies with GenericPrimitive data, the
        /// data is set to GenericComposite (this avoids the creation of a chain of
        /// ExtraComponents on GenericPrimitives).
        /// Components are numbered from 1.
        /// </summary>
        private static IType GetComponent(IType type, int comp)
        {
            IType ret = null;
            if (typeof(Varies).IsAssignableFrom(type.GetType()))
            {
                var v = (Varies)type;

                try
                {
                    if (comp > 1 && typeof(GenericPrimitive).IsAssignableFrom(v.Data.GetType()))
                    {
                        v.Data = new GenericComposite(v.Message);
                    }
                }
                catch (DataTypeException de)
                {
                    var message = "Unexpected exception copying data to generic composite: " + de.Message;
                    log.Error(message, de);
                    throw new ApplicationException(message);
                }

                ret = GetComponent(v.Data, comp);
            }
            else
            {
                if (typeof(IPrimitive).IsAssignableFrom(type.GetType()) && comp == 1)
                {
                    ret = type;
                }
                else if (typeof(GenericComposite).IsAssignableFrom(type.GetType()) ||
                            (typeof(IComposite).IsAssignableFrom(type.GetType()) && comp <= NumStandardComponents(type)))
                {
                    // note that GenericComposite can return components > number of standard components
                    try
                    {
                        ret = ((IComposite)type)[comp - 1];
                    }
                    catch (Exception e)
                    {
                        // TODO:  This may not be the write exception type:  Error() was originally thrown, but was not in project.
                        throw new ApplicationException(
                            "Internal error: HL7Exception thrown on getComponent(x) where x < # standard components.", e);
                    }
                }
                else
                {
                    ret = type.ExtraComponents.GetComponent(comp - NumStandardComponents(type) - 1);
                }
            }

            return ret;
        }

        private static int NumStandardComponents(IType t)
        {
            var n = 0;
            if (typeof(Varies).IsAssignableFrom(t.GetType()))
            {
                n = NumStandardComponents(((Varies)t).Data);
            }
            else if (typeof(IComposite).IsAssignableFrom(t.GetType()))
            {
                n = ((IComposite)t).Components.Length;
            }
            else
            {
                n = 1;
            }

            return n;
        }

        /// <summary>Gets path information from a path spec. </summary>
        private PathSpec ParsePathSpec(string spec)
        {
            var ps = new PathSpec(this);

            if (spec.StartsWith("."))
            {
                ps.Find = true;
                spec = spec.Substring(1);
            }
            else
            {
                ps.Find = false;
            }

            if (spec.Length == 0)
            {
                throw new HL7Exception("Invalid path (some path element is either empty or contains only a dot)");
            }

            var tok = new SupportClass.Tokenizer(spec, "()", false);
            ps.Pattern = tok.NextToken();
            if (tok.HasMoreTokens())
            {
                var repString = tok.NextToken();
                try
                {
                    ps.Rep = int.Parse(repString);
                }
                catch (FormatException)
                {
                    throw new HL7Exception(repString + " is not a valid rep #", ErrorCode.APPLICATION_INTERNAL_ERROR);
                }
            }
            else
            {
                ps.Rep = 0;
            }

            return ps;
        }

        /// <summary>Struct for information about a step in a segment path. </summary>
        private class PathSpec
        {
            public PathSpec(Terser enclosingInstance)
            {
                EnclosingInstance = enclosingInstance;
            }

            public Terser EnclosingInstance { get; }

            public string Pattern { get; set; }

            public bool IsGroup { get; set; }

            public bool Find { get; set; }

            public int Rep { get; set; }
        }
    }
}