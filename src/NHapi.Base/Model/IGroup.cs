/*
  The contents of this file are subject to the Mozilla Public License Version 1.1
  (the "License"); you may not use this file except in compliance with the License.
  You may obtain a copy of the License at http://www.mozilla.org/MPL/
  Software distributed under the License is distributed on an "AS IS" basis,
  WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for the
  specific language governing rights and limitations under the License.

  The Original Code is "Group.java".  Description:
  "An abstraction representing >1 message parts which may repeated together.

  An implementation of Group should enforce constraints about on the contents of the group
  and throw an exception if an attempt is made to add a Structure that the Group instance
  does not recognize.
  author: Bryan Tripp (bryan_tripp@sourceforge.net)"

  The Initial Developer of the Original Code is University Health Network. Copyright (C)
  2001.  All Rights Reserved.

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

namespace NHapi.Base.Model
{
    using System;

    /// <summary> An abstraction representing >1 message parts which may repeated together.
    /// An implementation of Group should enforce constraints about on the contents of the group
    /// and throw an exception if an attempt is made to add a Structure that the Group instance
    /// does not recognize.
    /// </summary>
    /// <author>  Bryan Tripp (bryan_tripp@sourceforge.net).
    /// </author>
    public interface IGroup : IStructure
    {
        /// <summary> Returns an ordered array of the names of the Structures in this
        /// Group.  These names can be used to iterate through the group using
        /// repeated calls to. <code>get(name)</code>.
        /// </summary>
        string[] Names { get; }

        /// <summary> Returns an array of Structure objects by name.  For example, if the Group contains
        /// an MSH segment and "MSH" is supplied then this call would return a 1-element array
        /// containing the MSH segment.  Multiple elements are returned when the segment or
        /// group repeats.  The array may be empty if no repetitions have been accessed
        /// yet using the get(...) methods.
        /// </summary>
        /// <throws>  HL7Exception if the named Structure is not part of this Group.  </throws>
        IStructure[] GetAll(string name);

        /// <summary> Returns the named structure.  If this Structure is repeating then the first
        /// repetition is returned.  Creates the Structure if necessary.
        /// </summary>
        /// <throws>  HL7Exception if the named Structure is not part of this Group.  </throws>
        IStructure GetStructure(string name);

        /// <summary> Returns a particular repetition of the named Structure. If the given repetition
        /// number is one greater than the existing number of repetitions then a new
        /// Structure is created.
        /// </summary>
        /// <throws>  HL7Exception if the named Structure is not part of this group,. </throws>
        /// <summary>    if the structure is not repeatable and the given rep is > 0,
        /// or if the given repetition number is more than one greater than the
        /// existing number of repetitions.
        /// </summary>
        IStructure GetStructure(string name, int rep);

        /// <summary> Returns true if the named structure is required. </summary>
        bool IsRequired(string name);

        /// <summary> Returns true if the named structure is repeating. </summary>
        bool IsRepeating(string name);

        /// <summary> Returns the Class of the Structure at the given name index.  </summary>
        Type GetClass(string name);

        [Obsolete("This method has been replaced by 'AddNonstandardSegment'.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:Element should begin with upper-case letter",
            Justification = "As this is a public member, we will duplicate the method and mark this one as obsolete.")]
        string addNonstandardSegment(string name);

        /// <summary>
        /// Expands the group definition to include a segment that is not
        /// defined by HL7 to be part of this group (eg: an unregistered Z segment).
        /// The new segment is slotted at the end of the group.  Thenceforward if
        /// such a segment is encountered it will be parsed into this location.
        /// If the segment name is unrecognized a GenericSegment is used.  The
        /// segment is defined as repeating and not required.
        /// </summary>
        string AddNonstandardSegment(string name);
    }
}