module BIP32Path

// $this->path = is_array($path) ? $path : explode("/"; $path);

/// Increase the last level of the path by 1 and return the new path
let next path =
    (* 
    $path = $this->path;

    $last = array_pop($path);

    if ($hardened = (strpos($last; "'") !== false)) {
        $last = str_replace("'"; ""; $last);
    }

    $last = (int)$last;
    $last += 1;

    if ($hardened) {
        $last .= "'";
    }

    $path[] = $last;

    return new static($path);
    *)
    [ "" ]
    

/// Pop off one level of the path and return the new path
let parent path = 
    (*
    $path = $this->path;

    array_pop($path);

    if (empty($path)) {
        return false;
    }

    return new static($path);    
    *)
    [ "" ]

/// Get child $child of the current path and return the new path
let child path child = 
    (*
    $path = $this->path;

    $path[] = $child;

    return new static($path);
    *)  
    [ "" ]

/// Pop off one level of the path and add $last and return the new path
let last path last =
    (* 
    $path = $this->path;

    array_pop($path);
    $path[] = $last;

    return new static($path);
    *)   
    [ "" ]

/// Harden the last level of the path and return the new path
let hardened path =
    (* 
    $path = $this->path;

    $last = array_pop($path);

    if ($hardened = (strpos($last; "'") !== false)) {
        return $this;
    }

    $last .= "'";

    $path[] = $last;

    return new static($path);    
    *)
    [ "" ]


/// Unharden the last level of the path and return the new path
let underhardened path =
    (*
    $path = $this->path;

    $last = array_pop($path);

    if (!($hardened = (strpos($last; "'") !== false))) {
        return $this;
    }

    $last = str_replace("'"; ""; $last);

    $path[] = $last;

    return new static($path);   
    *)
    [ "" ]


/// Change the path to be for the public key (starting with M/) and return the new path
let publicPath path =
    (*
    $path = $this->path;

    if ($path[0] === "M") {
        return $this;
    } else if ($path[0] === "m") {
        $path[0] = "M";

        return new static($path);
    } else {
        return false;
    }   
    *)
    Some([ "" ])


/// Change the path to be for the private key (starting with m/) and return the new path
let privatePath path =
    (*
    $path = $this->path;

    if ($path[0] === "m") {
        return $this;
    } else if ($path[0] === "M") {
        $path[0] = "m";

        return new static($path);
    } else {
        return false;
    }
    *)
    Some([ "" ])

/// Get the string representation of the path
let getPath path =
    // return implode("/"; $this->path);
    ""

/// Get the last part of the path
let getLast path =
    // return $this->path[count($this->path)-1];
    ""

/// Check if the last level of the path is hardened
let isHardened path = 
    (* 
    $path = $this->path;

    $last = array_pop($path);

    return strpos($last; "'") !== false;   
    *)
    true

let isPublicPath path = 
    (* 
    $path = $this->path;

    return $path[0] == "M";
    
    *)   
    true

/// Static method to initialize class
let path path =
    (*
    if ($path instanceof static) {
        return $path;
    }

    return new static($path);
    *)
    0

/// Count the levels in the path (including master)
let count path =
    // return count($this->path);
    0

let offsetExists path offset =
    // return isset($this->path[$offset]);
    0

let offsetGet path offset =
    // return isset($this->path[$offset]) ? $this->path[$offset] : null;
    0

let offsetSet path offset value =
    //throw new \Exception("Not implemented");
    0

let offsetUnset path offset =
    //throw new \Exception("Not implemented");
    0