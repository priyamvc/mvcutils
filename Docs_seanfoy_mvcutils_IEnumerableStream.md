
```
public class IEnumerableStream : System.IO.Stream
```

# Remarks #
To be added.

**Assembly:** seanfoy.mvcutils 0.0.10.0

# Members #
_List of all members,_

## Constructors ##
| `public IEnumerableStream (System.Collections.IEnumerable source, System.Text.Encoding enc);`  | To be added. |
|:-----------------------------------------------------------------------------------------------|:-------------|

## Properties ##
| `public override bool CanRead { get; }`  | To be added. |
|:-----------------------------------------|:-------------|
| `public override bool CanSeek { get; }`  | To be added. |
| `public override bool CanWrite { get; }`  | To be added. |
| `public System.Text.Encoding enc { set; get; }`  | To be added. |
| `public override long Length { get; }`   | To be added. |
| `public override long Position { set; get; }`  | To be added. |
| `public System.Collections.IEnumerator src { set; get; }`  | To be added. |

## Methods ##
| `protected override void Dispose (bool disposing);`  | To be added. |
|:-----------------------------------------------------|:-------------|
| `public override void Flush ();`                     | To be added. |
| `public override int Read (byte[] buffer, int offset, int count);`  | To be added. |
| `public override long Seek (long l, System.IO.SeekOrigin o);`  | To be added. |
| `public override void SetLength (long l);`           | To be added. |
| `public override void Write (byte[] b, int i, int j);`  | To be added. |


# Member Details #
_A detailed description of each member_

## Constructors ##
_A detrailed description of constructors._

### IEnumerableStream Constructor ###
_To be added._
```
public IEnumerableStream (System.Collections.IEnumerable source, System.Text.Encoding enc);
```

#### Remarks ####
To be added.

#### Parameters ####
  * System.Collections.IEnumerable `source`  To be added.
  * System.Text.Encoding `enc`  To be added.
## Properties ##
_A detailed description of properties._

### CanRead ###
_To be added._
```
public override bool CanRead { get; }
```
#### Value ####
To be added.

### CanSeek ###
_To be added._
```
public override bool CanSeek { get; }
```
#### Value ####
To be added.

### CanWrite ###
_To be added._
```
public override bool CanWrite { get; }
```
#### Value ####
To be added.

### enc ###
_To be added._
```
public System.Text.Encoding enc { set; get; }
```
#### Value ####
To be added.

### Length ###
_To be added._
```
public override long Length { get; }
```
#### Value ####
To be added.

### Position ###
_To be added._
```
public override long Position { set; get; }
```
#### Value ####
To be added.

### src ###
_To be added._
```
public System.Collections.IEnumerator src { set; get; }
```
#### Value ####
To be added.

## Methods ##
_A detailed description of methods._

### Dispose ###
_To be added._
```
protected override void Dispose (bool disposing);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Boolean `disposing`  To be added.

### Flush ###
_To be added._
```
public override void Flush ();
```
#### Remarks ####
To be added.

#### Parameters ####

### Read ###
_To be added._
```
public override int Read (byte[] buffer, int offset, int count);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Byte[.md](.md) `buffer`  To be added.
  * System.Int32 `offset`  To be added.
  * System.Int32 `count`  To be added.

### Seek ###
_To be added._
```
public override long Seek (long l, System.IO.SeekOrigin o);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Int64 `l`  To be added.
  * System.IO.SeekOrigin `o`  To be added.

### SetLength ###
_To be added._
```
public override void SetLength (long l);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Int64 `l`  To be added.

### Write ###
_To be added._
```
public override void Write (byte[] b, int i, int j);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Byte[.md](.md) `b`  To be added.
  * System.Int32 `i`  To be added.
  * System.Int32 `j`  To be added.