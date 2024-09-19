use master
go
drop database TracNghiemTiengAnhTHPT
go
create database TracNghiemTiengAnhTHPT
go
use TracNghiemTiengAnhTHPT
go
create table TaiKhoan
(
	Username varchar(30) PRIMARY KEY,
	[Password] varchar(30) NOT NULL,
	PhanQuyen nvarchar(30) NOT NULL,
	Gmail varchar(30) NOT NULL,
)

go
create table LienHe
(
	HoVaTen nvarchar(30) NOT NULL,
	Gmail varchar(30) NOT NULL,
	TieuDe nvarchar(30) NOT NULL,
	NoiDung nvarchar(300) NOT NULL,
)
create table NhomCauHoi
(
	MaNhom int NOT NULL primary key,
	NoiDung varchar(1000) NOT NULL,
)
create table CauHoi
(
	MaCauHoi varchar(20) NOT NULL PRIMARY KEY,
	NoiDung varchar(300) NOT NULL, 
	MaNhom int  CONSTRAINT FK_CauHoi_NhomCauHoi FOREIGN KEY (MaNhom)
    REFERENCES NhomCauHoi(MaNhom),
	MucDo int ,
	DapAnA varchar(100) NOT NULL,
	DapAnB varchar(100) NOT NULL,
	DapAnC varchar(100) NOT NULL,
	DapAnD varchar(100) NOT NULL,
	DapAnChinhXac varchar(10) NOT NULL,
	DaDuyet bit,
)
create table DangBai
(
	
	MaLoai int NOT NULL PRIMARY KEY,
	TenLoai varchar(30) NOT NULL,
)
create table ChiTietCauHoiDangBai
(
	MaCauHoi varchar(20)  CONSTRAINT FK_CauHoi_ChiTietCauHoiDangBai FOREIGN KEY (MaCauHoi)
    REFERENCES CauHoi(MaCauHoi),
	MaLoai int  CONSTRAINT FK_DangBai_ChiTietCauHoiDangBai FOREIGN KEY (MaLoai)
    REFERENCES DangBai(MaLoai) ,
	PRIMARY KEY(MaCauHoi,MaLoai )
)
create table KyThi
(
	MaDe varchar(20)  PRIMARY KEY,
	TenKyThi nvarchar(100) NOT NULL,
	ThoiGian int NOT NULL,
	ThoiGianBatDau datetime ,
	ThoiGianKetThuc datetime ,
	CongKhai bit not null,
)
create table BaoLoi
(
	NoiDung nvarchar(300) NOT NULL,
	Username varchar(30) CONSTRAINT FK_BaoLoi_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	MaCauHoi varchar(20) CONSTRAINT FK_BaoLoi_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES CauHoi(MaCauHoi) ,
	MaDe varchar(20) NOT NULL,
	PRIMARY KEY(Username,MaCauHoi,MaDe) 
)
create table PhongThi
(
	MaPhong int NOT NULL PRIMARY KEY,
	MatKhau varchar(30),
	TenPhong nvarchar(100) NOT NULL,
)
create table PhongThiKyThi
(
	MaDe varchar(20)   CONSTRAINT FK_PhongThiKyThi_KyThi FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	MaPhong int CONSTRAINT FK_PhongThiKyThi_PhongThi FOREIGN KEY (MaPhong) REFERENCES PhongThi(MaPhong) ,
	 PRIMARY KEY(MaDe,MaPhong)
)
create table KyThiCauHoi
(
	MaDe varchar(20)  CONSTRAINT FK_KyThiCauHoi_KyThi FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	MaCauHoi varchar(20) CONSTRAINT FK_KyThiCauHoi_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES Cauhoi(MaCauHoi) ,
	PRIMARY KEY(MaDe,MaCauHoi)
)
create table PhongThiNguoiDung
(
	Username varchar(30) CONSTRAINT FK_PhongThiNguoiDung_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	MaPhong int CONSTRAINT FK_PhongThiNguoiDung_PhongThi FOREIGN KEY (MaPhong) REFERENCES PhongThi(MaPhong),
	PRIMARY KEY(Username,MaPhong)
)

create table KetQua
(
	Username varchar(30) CONSTRAINT FK_KetQua_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	MaDe varchar(20) CONSTRAINT FK_KetQua_KyThi FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	Diem float,
	PRIMARY KEY(Username,MaDe)
)
create table ChiTietKetQua
(
	Username varchar(30) CONSTRAINT FK_ChiTietKetQua_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	MaDe varchar(20) CONSTRAINT FK_ChiTietKetQua_KyThi FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	MaCauHoi varchar(20) CONSTRAINT FK_ChiTietKetQua_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES Cauhoi(MaCauHoi) ,
	DapAnChon char NOT NULL,
	PRIMARY KEY(Username,MaDe,MaCauHoi)
)

create table DanhGia
(
	Username varchar(30) CONSTRAINT FK_DanhGia_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	Rate int NOT NULL,
	NoiDung nvarchar(100) NOT NULL,
	MaDe varchar(20)  CONSTRAINT FK_KyThiCauHoi_MaDe FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	PRIMARY KEY(Username,MaDe)
)

insert into TaiKhoan values ('admin','123','admin','d@gmail.com')
insert into TaiKhoan values ('hocsinh','123','hocsinh','d@gmail.com')
insert into TaiKhoan values ('giaovien','123','giaovien','d@gmail.com')
insert into KyThi values('123','Test so 1',90,null,null,1)
insert into KyThi values('1','Test so 2',90,null,null,1)
insert into KyThi values('12','Test so 2',90,null,null,1)
insert into KyThi values('1234','Test so 3',90,null,null,1)
