use master
go
drop database TracNghiemTiengAnhTHPT
go

go
create database TracNghiemTiengAnhTHPT
go
use TracNghiemTiengAnhTHPT
go
create table TaiKhoan
(
	Username varchar(30) PRIMARY KEY,
	[Password] varchar(1000) NOT NULL,
	PhanQuyen nvarchar(30) NOT NULL,
	Gmail varchar(30) NOT NULL,
	isDeleted bit,
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
	MaNhom int IDENTITY(1,1) primary key,
	NoiDung varchar(1000) NOT NULL,
)
create table CauHoi
(
	MaCauHoi int IDENTITY(1,1) PRIMARY KEY,
	NoiDung varchar(4000) NOT NULL, 
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
	MaLoai int IDENTITY(1,1) PRIMARY KEY,
	TenLoai varchar(30) NOT NULL,
)
create table ChiTietCauHoiDangBai
(
	MaCauHoi int CONSTRAINT FK_CauHoi_ChiTietCauHoiDangBai FOREIGN KEY (MaCauHoi)
    REFERENCES CauHoi(MaCauHoi),
	MaLoai int  CONSTRAINT FK_DangBai_ChiTietCauHoiDangBai FOREIGN KEY (MaLoai)
    REFERENCES DangBai(MaLoai) ,
	PRIMARY KEY(MaCauHoi,MaLoai )
)

create table KyThi
(
	MaDe int IDENTITY(1,1) PRIMARY KEY,
	TenKyThi nvarchar(100) NOT NULL,
	ThoiGian int NOT NULL,
	ThoiGianBatDau datetime ,
	ThoiGianKetThuc datetime ,
	CongKhai bit not null,
	isDeleted bit,
)
create table BaoLoi
(
	NoiDung nvarchar(300) NOT NULL,
	Username varchar(30) CONSTRAINT FK_BaoLoi_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	MaCauHoi int CONSTRAINT FK_BaoLoi_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES CauHoi(MaCauHoi) ,
	MaDe int CONSTRAINT FK_KyThi_BaoLoi FOREIGN KEY (MaDe)  REFERENCES KyThi(MaDe),
	PRIMARY KEY(Username,MaCauHoi,MaDe) 
)
create table PhongThi
(
	MaPhong int IDENTITY(1,1) PRIMARY KEY,
	MatKhau varchar(30),
	TenPhong nvarchar(100) NOT NULL,
)
create table PhongThiKyThi
(
	MaDe int   CONSTRAINT FK_PhongThiKyThi_KyThi FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	MaPhong int CONSTRAINT FK_PhongThiKyThi_PhongThi FOREIGN KEY (MaPhong) REFERENCES PhongThi(MaPhong) ,
	 PRIMARY KEY(MaDe,MaPhong)
)
create table KyThiCauHoi
(
	MaDe int  CONSTRAINT FK_DeThiCauHoi_KyThi FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	MaCauHoi int CONSTRAINT FK_DeThiCauHoi_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES Cauhoi(MaCauHoi) ,
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
	Maketqua int IDENTITY(1,1) PRIMARY KEY,
	Username varchar(30) CONSTRAINT FK_KetQua_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	MaDe int CONSTRAINT FK_KetQua_KyThi FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	Diem float,
	
)
create table ChiTietKetQua
(
	Username varchar(30) CONSTRAINT FK_ChiTietKetQua_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	Maketqua int CONSTRAINT FK_ChiTietKetQua_Maketqua FOREIGN KEY (Maketqua) REFERENCES KetQua(Maketqua) ,
	MaCauHoi int CONSTRAINT FK_ChiTietKetQua_CauHoi FOREIGN KEY (MaCauHoi) REFERENCES Cauhoi(MaCauHoi) ,
	DapAnChon char NOT NULL,
	PRIMARY KEY(Username,Maketqua,MaCauHoi)
)

create table DanhGia
(
	Username varchar(30) CONSTRAINT FK_DanhGia_TaiKhoan FOREIGN KEY (Username) REFERENCES TaiKhoan(Username),
	Rate int NOT NULL,
	NoiDung nvarchar(100) NOT NULL,
	MaDe int  CONSTRAINT FK_KyThiCauHoi_MaDe FOREIGN KEY (MaDe) REFERENCES KyThi(MaDe) ,
	PRIMARY KEY(Username,MaDe)
)


insert into TaiKhoan values ('admin','123','admin','d@gmail.com',0)
insert into TaiKhoan values ('hocsinh','123','hocsinh','d@gmail.com',0)
insert into TaiKhoan values ('giaovien','123','giaovien','d@gmail.com',0)
insert into KyThi values('Test so 1',90,null,null,1,0)
insert into KyThi values('Test so 2',90,null,null,1,0)
insert into KyThi values('Test so 2',90,null,null,1,0)
insert into KyThi values('Test so 3',90,null,null,1,0)
insert into KyThi values(N'Ph?n nghe',90,null,null,1,0)
go
insert into DanhGia values ('admin',3,'good',1)
insert into DanhGia values ('admin',2,'good',2)
insert into DanhGia values ('admin',5,'good',3)
go

insert into NhomCauHoi values('Listen to some information about a student’s health and habits. Circle the best answer A, B, or C. You will listen TWICE.')
ALTER TABLE NhomCauHoi
ALTER COLUMN NoiDung varchar(8000) NOT NULL;
INSERT INTO NhomCauHoi (NoiDung)
VALUES 
( 'Read the passage. Circle the best answer A, B, or C to each of the questions.Is the generation gap in America no longer a severe problem as it used to be? Dating back to the 1960s when teenagers tended to lash out the values and goals of their parents as well as rebel against the authority figures, the incendiary conflicts between older and younger generations increased sharply. It’s because teenagers ’ world wasn’t any longer limited in a narrow society that wasn’t mobile, and instead of going to church every weekend, they were exposed to various forms of social media like television and radios. They got access to huge sources of new ideas, which liberated them from old-fashioned and boring lifestyles whereas many older people were conservative and didn’t accept differences disturbing their normal life. Over time, however, the tension between generations has been alleviated due to the improved mutual understanding of baby boomers, Millennials and even Zillennials in many fields of life. According to recent research, the largest generational discrepancies between young and old in the United States are the use of technology and taste in music. Nevertheless, in terms of technological use, many older people gradually learn how to use a laptop or smartphone to surf the Internet from their children and especially their grandchildren due to their recognition of huge technological benefits. Regarding the musical differences, unlike in the past, older generations nowadays appear to give fewer critical remarks on what type of music teens should listen to and begin to accept the variety of music tastes when living under the same roofs.')
insert into NhomCauHoi values('Choose the best option to complete each sentence. Circle A, B, C or D.')
go

INSERT INTO CauHoi (NoiDung, MaNhom, MucDo, DapAnA, DapAnB, DapAnC, DapAnD, DapAnChinhXac, DaDuyet)
VALUES 
('John’s parents are very strict, so he ____________ use his smartphone after 9 PM.', 3, 1, 
 'couldn’t', 'didn’t', 'mustn’t', 'doesn’t have to', 'C', 1);

INSERT INTO CauHoi ( NoiDung, MaNhom, MucDo, DapAnA, DapAnB, DapAnC, DapAnD, DapAnChinhXac, DaDuyet)
VALUES 
('She’s thinking of moving out of her apartment ____________ the current rent is too high.',3, 1, 
 'although', 'in spite of', 'because', 'due to', 'C', 1);

INSERT INTO CauHoi (NoiDung, MaNhom, MucDo, DapAnA, DapAnB, DapAnC, DapAnD, DapAnChinhXac, DaDuyet)
VALUES 
('Although Kim moved here 2 years ago, she still hasn’t explored all of her ____________.', 3, 1, 
 'adulthood', 'childhood', 'livelihood', 'neighbourhood', 'D', 1);
 select * from NhomCauHoi
INSERT INTO CauHoi ( NoiDung, MaNhom, MucDo, DapAnA, DapAnB, DapAnC, DapAnD, DapAnChinhXac, DaDuyet)
VALUES 
('What is the passage mainly about?', 4, 1, 
 'The development of the generation gap in America nowadays.', 
 'The alleviation of the generation gap in America nowadays.', 
 'The eradication of the generation gap in America in the past.', 
 'The reduction of the generation gap in America in the past.', 'A', 1);

INSERT INTO CauHoi (NoiDung, MaNhom, MucDo, DapAnA, DapAnB, DapAnC, DapAnD, DapAnChinhXac, DaDuyet)
VALUES 
('According to the passage, what types of social media provided young people with new ideas in the 1960s?', 4, 1, 
 'The Internet and television.', 
 'The Internet and radios.', 
 'The television and printed newspaper.', 
 'The television and radios.', 'D', 1);


INSERT INTO CauHoi (NoiDung, MaNhom, MucDo, DapAnA, DapAnB, DapAnC, DapAnD, DapAnChinhXac, DaDuyet)
VALUES 
('Why did the older generation refuse to access new things?', 4, 1, 
 'Because they thought those things were tedious.', 
 'Because they loved to go to church every weekend.', 
 'Because they didn’t want to change their normal life.', 
 'Because they weren’t able to learn technological devices.', 'C', 1);


INSERT INTO CauHoi (NoiDung, MaNhom, MucDo, DapAnA, DapAnB, DapAnC, DapAnD, DapAnChinhXac, DaDuyet)
VALUES 
('What can be inferred from the passage?', 4, 1, 
 'Nowadays there is no longer a generation gap.', 
 'The generation gap didn’t remain after the 1960s.', 
 'Teaching older people to use modern devices can bridge the gap between generations.', 
 'Younger Americans are forced to change their taste in music nowadays.', 'C', 1);
 go
-- Inserting data into KyThiCauHoi

-- Linking questions Q001 to Q004 with test 123
select * from CauHoi
INSERT INTO KyThiCauHoi (MaDe, MaCauHoi)
VALUES 
(1, 1),
(1, 2),
(1, 3)

create view ViewChitietKyThi_2 as
(
SELECT 
    k.MaDe ,
	k.TenKyThi ,
	ThoiGian ,
	ThoiGianBatDau ,
	ThoiGianKetThuc ,
	CongKhai ,
    -- Count the number of questions for the test, handling cases where there are no questions
    COALESCE(COUNT(kch.MaCauHoi), 0) AS SoCauHoi,
    -- Average rating, handling cases where there is no rating
    COALESCE(AVG(dg.rate), 0) AS DiemTrungBinh,
	COALESCE(COUNT(KetQua.Diem), 0) AS Soluot
FROM 
    KyThi k
    -- Left join KyThiCauHoi to include tests even if they have no questions
    LEFT JOIN KyThiCauHoi kch ON k.MaDe = kch.MaDe
    -- Left join DanhGia to include tests even if they have no ratings
    LEFT JOIN DanhGia dg ON k.MaDe = dg.MaDe
	LEFT JOIN KetQua  ON k.MaDe = KetQua.MaDe
GROUP BY 
    k.MaDe ,
	k.TenKyThi ,
	ThoiGian ,
	ThoiGianBatDau ,
	ThoiGianKetThuc ,
	CongKhai 
)

ALTER table PhongThi
add malop int
ALTER TABLE TaiKhoan
ADD status BIT NOT NULL DEFAULT 0;
ALTER TABLE CauHoi
ADD isDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE NhomCauHoi
ADD isDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE ChiTietKetQua
ADD MaDe int;
ALTER TABLE CauHoi
ADD UsernameTacGia varchar(30);
ALTER TABLE KyThi
ADD UsernameTacGia varchar(30);
