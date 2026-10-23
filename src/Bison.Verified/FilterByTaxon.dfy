module Bison {
    module Models {
        class {:extern} Taxon {
            function {:extern} IsSubTaxon(ancestor: Taxon): bool
        }
        
        class {:extern} Observation {
            function {:extern} {:axiom} GetTaxon(): Taxon
        }
    }

    module Verified {
        import M = Models
        method FilterBy(root: M.Taxon, obs: seq<M.Observation>) returns (filtered: seq<M.Observation>)
            ensures forall obs :: obs in filtered ==> obs.GetTaxon().IsSubTaxon(root)
        {
            if |obs| == 0 {
                return [];
            }
            else if obs[0].GetTaxon().IsSubTaxon(root){
                var extra := FilterBy(root, obs[1..]);
                return [obs[0]] + extra;
            }
            else {
                var filtered := FilterBy(root, obs[1..]);
                return filtered;
            }
        }
    }
}
