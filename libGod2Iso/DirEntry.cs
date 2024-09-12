namespace libGod2Iso;

internal class DirEntry(int sector, int length) {
    
    public int Sector = sector;
    public int Length = length;
    
    public long StartPos() {
        return Sector * 2048L;
    }
    public long EndPos() {
        return (Sector * 2048L) + Length;
    }
}