USE [BetterCms]
GO
/****** Object:  StoredProcedure [dbo].[spAddBlogCategory]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spAddBlogCategory]
@Id uniqueidentifier,
@Version int,
@IsDeleted bit,
@PageId uniqueidentifier,
@CategoryId uniqueidentifier
As
Begin

declare @_Id uniqueidentifier 
set @_Id =NEWID()
 if exists(select Id from bcms_pages.PageCategories where Id=@Id)
 Begin
 update bcms_pages.PageCategories
 set Version=@Version,IsDeleted=@IsDeleted,ModifiedOn=getutcdate(),ModifiedByUser='admin',PageId=@PageId,CategoryId=@CategoryId
 where bcms_pages.PageCategories.Id=@Id
 select @Id
 End
 else
  Begin
	Insert into bcms_pages.PageCategories(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,PageId,CategoryId) 
	values(@_Id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@PageId,@CategoryId)
	Select @_Id
  End
End 

GO
/****** Object:  StoredProcedure [dbo].[spAddBlogPageContent]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spAddBlogPageContent]
@Id uniqueidentifier,
@Version int,
@IsDeleted bit,
@PageId uniqueidentifier,
@ContentId uniqueidentifier,
@RegionId uniqueidentifier,
@Order int
As
Begin

declare @_Id uniqueidentifier 
set @_Id =NEWID()
 if exists(select Id from bcms_root.PageContents where  Id=@Id)
 Begin
    update bcms_root.PageContents
    set Version=@Version,IsDeleted=@IsDeleted,ModifiedOn=getutcdate(),ModifiedByUser='admin',PageId=@PageId,ContentId=@ContentId,RegionId=@RegionId,[Order]=@Order
    where Id=@Id
	Select @Id
 End
 else
  Begin
    if(@Id='00000000-0000-0000-0000-000000000000')
	Begin
	Insert into bcms_root.PageContents(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,PageId,ContentId,RegionId,[Order]) 
	values(@_Id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@PageId,@ContentId,@RegionId,@Order)
	Select @_Id
	End
	else
	Begin
	Insert into bcms_root.PageContents(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,PageId,ContentId,RegionId,[Order]) 
	values(@Id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@PageId,@ContentId,@RegionId,@Order)
	Select @Id
	End
  End
End 

GO
/****** Object:  StoredProcedure [dbo].[spAddBlogPostContent]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spAddBlogPostContent]
@Id uniqueidentifier,
@ActivationdDate datetime,
@ExpirationDate datetime,
@CustomCss nvarchar(MAX),
@UseCustomCss bit,
@CustomJs nvarchar(MAX),
@UseCustomJs bit,
@Html nvarchar(MAX),
@EditInSourceMode bit,
@OriginalText nvarchar(MAX),
@ContentTextMode int,
@Version int,
@IsDeleted bit,
@Name nvarchar(200),
@PreviewUrl varchar(850),
@Status int,
@PublishedOn datetime,
@PublishedByUser nvarchar(200),
@OriginalId uniqueidentifier

As
Begin

declare @_Id uniqueidentifier 
declare @flag int
set @_Id =NEWID()
set @flag =1


if(@flag = 1)
   Begin
     if exists(select Id from bcms_root.Contents where  Id=@Id)
	 Begin
	 update bcms_root.Contents
	 set Version=@Version,IsDeleted=@IsDeleted,ModifiedOn=getutcdate(),ModifiedByUser='admin',Name=@Name,PreviewUrl=@PreviewUrl,Status=@Status,PublishedOn=@PublishedOn,PublishedByUser=@PublishedByUser,OriginalId=@OriginalId
	 where bcms_root.Contents.Id=@Id and bcms_root.Contents.IsDeleted<>1
	 select @Id
	 End
	 else
	 Begin
	   if(@Id='00000000-0000-0000-0000-000000000000')
	     Begin
	       Insert into bcms_root.Contents(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,Name,PreviewUrl,Status,PublishedOn,PublishedByUser,OriginalId)
           values(@_Id ,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@Name,@PreviewUrl,@Status,@PublishedOn,@PublishedByUser,@OriginalId)
	       Select @_Id 
	     End
	   else
	     Begin
		    Insert into bcms_root.Contents(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,Name,PreviewUrl,Status,PublishedOn,PublishedByUser,OriginalId)
            values(@Id ,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@Name,@PreviewUrl,@Status,@PublishedOn,@PublishedByUser,@OriginalId)
	        Select @Id 
		 End
     End

   End
if(@flag = 1)
  Begin
  if exists(select Id from bcms_pages.HtmlContents where  Id=@Id)
  Begin
  update bcms_pages.HtmlContents 
  set ActivationDate=@ActivationdDate,ExpirationDate=@ExpirationDate,CustomCss=@CustomCss,UseCustomCss=@UseCustomCss,CustomJs=@CustomJs,UseCustomJs=@UseCustomJs,Html=@Html,EditInSourceMode=@EditInSourceMode,OriginalText=@OriginalText,ContentTextMode=@ContentTextMode
  where bcms_pages.HtmlContents.Id=@Id 
  select @Id
  End
  else
  Begin
  if(@Id='00000000-0000-0000-0000-000000000000')
  Begin
    Insert into bcms_pages.HtmlContents(Id,ActivationDate,ExpirationDate,CustomCss,UseCustomCss,CustomJs,UseCustomJs,Html,EditInSourceMode,OriginalText,ContentTextMode)
    values(@_Id ,@ActivationdDate,@ExpirationDate,@CustomCss,@UseCustomCss,@CustomJs,@UseCustomJs,@Html,@EditInSourceMode,@OriginalText,@ContentTextMode)
    Select @_Id 
  End
  else
   Begin
     Insert into bcms_pages.HtmlContents(Id,ActivationDate,ExpirationDate,CustomCss,UseCustomCss,CustomJs,UseCustomJs,Html,EditInSourceMode,OriginalText,ContentTextMode)
     values(@Id ,@ActivationdDate,@ExpirationDate,@CustomCss,@UseCustomCss,@CustomJs,@UseCustomJs,@Html,@EditInSourceMode,@OriginalText,@ContentTextMode)
     Select @Id 
   End
  End
  
  End
if(@flag = 1)
  Begin
   if exists(select Id from bcms_blog.BlogPostContents where  Id=@Id)
   Begin
   select @Id
   End
   else
   Begin
    if(@Id='00000000-0000-0000-0000-000000000000')
    Begin
     Insert into bcms_blog.BlogPostContents(Id) 
	 values(@_Id )
	 Select @_Id 
	End
	else
	 Begin
	  Insert into bcms_blog.BlogPostContents(Id) 
	  values(@Id )
	  Select @Id 
	 End
   End
  End

End

GO
/****** Object:  StoredProcedure [dbo].[spAddBlogPosts]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spAddBlogPosts]
@Id uniqueidentifier,
@AuthorId uniqueidentifier,
@ActivationDate datetime,
@ExpirationDate datetime,
@Version int,
@IsDeleted bit,
@PageUrl nvarchar(850),
@Title nvarchar(200),
@LayoutId uniqueidentifier,
@PublishedOn datetime,
@MetaTitle nvarchar(200),
@MetaKeywords nvarchar(MAX),
@MetaDescription nvarchar(MAX),
@Status int,
@pageUrlHash varchar(32),
@masterPageId uniqueidentifier,
@isMasterPage bit,
@languageId uniqueidentifier,
@languageGroupIdentifier uniqueidentifier,
@forceAccessProtocol int,
@Description nvarchar(2000),
@ImageId uniqueidentifier,
@CustomCss nvarchar(MAX),
@CustomJs nvarchar(MAX),
@UseCanonicalUrl bit,
@UseNoFollow bit,
@UseNoIndex bit,
@CatergoryId uniqueidentifier,
@SecondaryImageId uniqueidentifier,
@FeaturedImageId uniqueidentifier,
@IsArchived bit

As
Begin
declare @_Id uniqueidentifier 
set @_Id =NEWID()
declare @flag int
set @flag =1
  if(@flag = 1)
  Begin
  if exists(select Id from bcms_root.Pages  where  Id=@Id)
   Begin
      update bcms_root.Pages  
	  set Version=@Version,IsDeleted=@IsDeleted,ModifiedOn=getutcdate(),ModifiedByUser ='admin',PageUrl=@PageUrl,Title=@Title,LayoutId=@LayoutId,PublishedOn=@PublishedOn,MetaTitle=@MetaTitle,MetaKeywords=@MetaKeywords,MetaDescription=@MetaDescription,Status=@Status,PageUrlHash=@pageUrlHash,MasterPageId=@masterPageId,IsMasterPage=@isMasterPage,LanguageId=@languageId,LanguageGroupIdentifier=@languageGroupIdentifier,ForceAccessProtocol=@forceAccessProtocol
	  where bcms_root.Pages.Id=@Id
	  Select @Id
    End
   else
   Begin
    if(@Id='00000000-0000-0000-0000-000000000000')
	 Begin
	  Insert into bcms_root.Pages(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,PageUrl,Title,LayoutId,PublishedOn,MetaTitle,MetaKeywords,MetaDescription,Status,PageUrlHash,MasterPageId,IsMasterPage,LanguageId,LanguageGroupIdentifier,ForceAccessProtocol)
	  values(@_Id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@PageUrl,@Title,@LayoutId,@PublishedOn,@MetaTitle,@MetaKeywords,@MetaDescription,@Status,@pageUrlHash,@masterPageId,@isMasterPage,@languageId,@languageGroupIdentifier,@forceAccessProtocol)
	  Select @_Id
	  End
	else
	  Begin
	    Insert into bcms_root.Pages(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,PageUrl,Title,LayoutId,PublishedOn,MetaTitle,MetaKeywords,MetaDescription,Status,PageUrlHash,MasterPageId,IsMasterPage,LanguageId,LanguageGroupIdentifier,ForceAccessProtocol)
	    values(@Id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@PageUrl,@Title,@LayoutId,@PublishedOn,@MetaTitle,@MetaKeywords,@MetaDescription,@Status,@pageUrlHash,@masterPageId,@isMasterPage,@languageId,@languageGroupIdentifier,@forceAccessProtocol)
	    Select @Id
	  End
   End 
  End
 
 if(@flag = 1)
     Begin
	 if exists(select Id from bcms_pages.Pages  where  Id=@Id)
	 Begin
	 update bcms_pages.Pages
	 set Description=@Description,ImageId=@ImageId,CustomCss=@CustomCss,CustomJS=@CustomJs,UseCanonicalUrl=@UseCanonicalUrl,UseNoFollow=@UseNoFollow,UseNoIndex=@UseNoIndex,CategoryId=@CatergoryId,SecondaryImageId=@SecondaryImageId,FeaturedImageId=@FeaturedImageId,IsArchived=@IsArchived
	 where bcms_pages.Pages.Id =@Id
	 Select @Id
	 End
	 else
	 Begin
	 if(@Id='00000000-0000-0000-0000-000000000000')
	 Begin
	   Insert into bcms_pages.Pages(Id,Description,ImageId,CustomCss,CustomJS,UseCanonicalUrl,UseNoFollow,UseNoIndex,CategoryId,SecondaryImageId,FeaturedImageId,IsArchived)
	   values(@_Id,@Description,@ImageId,@CustomCss,@CustomJs,@UseCanonicalUrl,@UseNoFollow,@UseNoIndex,@CatergoryId,@SecondaryImageId,@FeaturedImageId,@IsArchived)
	   Select @_Id
	 End
	 else
	  Begin
	   Insert into bcms_pages.Pages(Id,Description,ImageId,CustomCss,CustomJS,UseCanonicalUrl,UseNoFollow,UseNoIndex,CategoryId,SecondaryImageId,FeaturedImageId,IsArchived)
	   values(@Id,@Description,@ImageId,@CustomCss,@CustomJs,@UseCanonicalUrl,@UseNoFollow,@UseNoIndex,@CatergoryId,@SecondaryImageId,@FeaturedImageId,@IsArchived)
	   Select @Id
	  End
	 End
	 End
 if(@flag=1)
    Begin
	if exists(select Id from bcms_blog.BlogPosts  where  Id=@Id)
	 Begin
	 update bcms_blog.BlogPosts
	 set AuthorId=@AuthorId,ActivationDate=@ActivationDate,ExpirationDate=@ExpirationDate
	 where bcms_blog.BlogPosts.Id =@Id
	 Select @Id
	 End
	else
	 Begin
	  if(@Id='00000000-0000-0000-0000-000000000000')
	   Begin
	     Insert into bcms_blog.BlogPosts(Id,AuthorId,ActivationDate,ExpirationDate) 
	     values(@_Id,@AuthorId,@ActivationDate,@ExpirationDate)
	     Select @_Id
	   End
	  Else
	     Begin
		 Insert into bcms_blog.BlogPosts(Id,AuthorId,ActivationDate,ExpirationDate) 
	     values(@Id,@AuthorId,@ActivationDate,@ExpirationDate)
	     Select @Id
		 End
	 End
    End
End 

GO
/****** Object:  StoredProcedure [dbo].[spAddNewAuthor]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spAddNewAuthor]
@Id uniqueidentifier,
@Version int,
@IsDeleted bit,
@Name nvarchar(100),
@ImageId uniqueidentifier,
@Description nvarchar(500)

As
Begin

declare @_Id uniqueidentifier 
set @_Id =NEWID()

	Insert into bcms_blog.Authors(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,Name,ImageId,Description) 
	values(@_Id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@Name,@ImageId,@Description)
	Select @_Id
End 
GO
/****** Object:  StoredProcedure [dbo].[spaddnewrole]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spaddnewrole]
@id uniqueidentifier,
@version int,
@name varchar(100),
@description varchar(100)
As
Begin

declare @_id uniqueidentifier 
set @_id =NEWID()
--select * from bcms_users.Roles
	Insert into bcms_users.Roles(Id,Name,Description,version,ModifiedByUser,ModifiedOn,CreatedByUser,CreatedOn) 
	values(@_id,@name,@description,@version,'admin',getutcdate(),'admin',getutcdate())
	Select @_id
End
GO
/****** Object:  StoredProcedure [dbo].[spAddRedirect]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spAddRedirect]
@Id uniqueidentifier,
@Version int,
@IsDeleted bit,
@PageUrl nvarchar(850),
@RedirectUrl nvarchar(850)
As
Begin

declare @_Id uniqueidentifier 
set @_Id =NEWID()
 
  Begin
     if exists(select Id from bcms_pages.Redirects where  Id=@Id)
	 Begin
	    update bcms_pages.Redirects
	    set Version=@Version,IsDeleted=@IsDeleted,CreatedOn=getutcdate(),CreatedByUser='admin',ModifiedOn=getutcdate(),ModifiedByUser='admin',PageUrl=@PageUrl,RedirectUrl=@RedirectUrl
	    where bcms_pages.Redirects.Id=@Id and bcms_pages.Redirects.IsDeleted<>1
	    select @Id
	 End
	 else
	 Begin
	    Insert into bcms_pages.Redirects(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,PageUrl,RedirectUrl) 
	    values(@_Id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate(),'admin',@PageUrl,@RedirectUrl)
	    Select @_Id
	End
  End
End 
GO
/****** Object:  StoredProcedure [dbo].[spAddUserRole]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spAddUserRole]
@UserId uniqueidentifier,
@RoleId uniqueidentifier

As
Begin
	if exists(select id from bcms_users.UserRoles  where UserId = @UserId  and RoleId=@RoleId)
	Begin
		select 0 as result
	End
	else
	Begin
	   declare @_id uniqueidentifier 
       set @_id =NEWID()
      --select * from bcms_users.Roles
	   Insert into bcms_users.UserRoles(Id,version,ModifiedByUser,ModifiedOn,CreatedByUser,CreatedOn,RoleId,UserId) 
	   values(@_id,'1','admin',getutcdate(),'admin',getutcdate(),@RoleId,@UserId)
	   select 1 as result
	End

End


GO
/****** Object:  StoredProcedure [dbo].[spBlogExistingCategoryIds]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spBlogExistingCategoryIds]

As
Begin

 select  c.Id from bcms_root.Categories as c 
 
End 
GO
/****** Object:  StoredProcedure [dbo].[spBlogLayoutRegionDetailsForLayout]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spBlogLayoutRegionDetailsForLayout]
@LayoutId uniqueidentifier


As
Begin

 select l.Id,l.Version,l.IsDeleted,l.CreatedOn,l.CreatedByUser,l.ModifiedOn,l.ModifiedByUser,l.DeletedOn,l.DeletedByUser,l.Description,l.RegionId
 from bcms_root.LayoutRegions as l where l.LayoutId =@LayoutId 

End 

GO
/****** Object:  StoredProcedure [dbo].[spBlogPageContentDetailsWithPageId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spBlogPageContentDetailsWithPageId]
@PageId uniqueidentifier


As
Begin

 select p.Id,p.Version,p.IsDeleted,p.CreatedOn,p.CreatedByUser,p.ModifiedOn,p.ModifiedByUser,p.DeletedOn,p.DeletedByUser,p.ContentId,p.RegionId,p.[Order]
 from bcms_root.PageContents as p where p.PageId =@PageId and p.IsDeleted <> 1

End 
GO
/****** Object:  StoredProcedure [dbo].[spBlogRegionPageContentDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spBlogRegionPageContentDetails]
@RegionId uniqueidentifier


As
Begin

 select p.Id,p.Version,p.IsDeleted,p.CreatedOn,p.CreatedByUser,p.ModifiedOn,p.ModifiedByUser,p.DeletedOn,p.DeletedByUser,p.PageId,p.ContentId,p.[Order]
 from bcms_root.PageContents as p where p.RegionId =@RegionId and p.IsDeleted <> 1

End 
GO
/****** Object:  StoredProcedure [dbo].[spDeleteAuthor]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spDeleteAuthor]
@Id uniqueidentifier
As
Begin
	Begin Try
		Update bcms_blog.Authors  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where Id=@Id 
		select bcms_blog.Authors.IsDeleted,bcms_blog.Authors.Version from bcms_blog.Authors where Id=@Id
	End Try
	Begin Catch
		select 0 as result
	End Catch
		
End
GO
/****** Object:  StoredProcedure [dbo].[spDeleteFile]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spDeleteFile]
@id uniqueidentifier
As
Begin
	Begin Try
		Update bcms_media.Medias set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where id=@id
		select 1 as result
	End Try
	BEgin Catch
		select 0 as result
	End Catch
		
End

GO
/****** Object:  StoredProcedure [dbo].[spDeleteRole]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spDeleteRole]
@UserId uniqueidentifier
As
Begin
	Begin Try
		Update bcms_users.UserRoles  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where UserId=@UserId 
		select 1 as result
	End Try
	BEgin Catch
		select 0 as result
	End Catch
		
End
GO
/****** Object:  StoredProcedure [dbo].[spDeleteUser]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spDeleteUser]
@UserId uniqueidentifier
As
Begin
	Begin Try
		Update bcms_users.Users set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where id=@UserId
		select 1 as result
	End Try
	BEgin Catch
		select 0 as result
	End Catch
		
End

GO
/****** Object:  StoredProcedure [dbo].[spDeleteUserRole]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spDeleteUserRole]
@UserId uniqueidentifier

As
Begin
	Begin Try
		Update bcms_users.UserRoles set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where id=@UserId
		select 1 as result
	End Try
	BEgin Catch
		select 0 as result
	End Catch
		
End

GO
/****** Object:  StoredProcedure [dbo].[spGetAccessRuleIds]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetAccessRuleIds]
@PageId uniqueidentifier


As
Begin

 if exists(select p.PageId from bcms_root.PageAccessRules as p where  p.PageId=@PageId)
 Begin
 select p.AccessRuleId
 from bcms_root.PageAccessRules as p  where p.PageId= @PageId 
 End
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetAccessRules]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetAccessRules]
@AccessRuleId uniqueidentifier


As
Begin

 
 select a.Id,a.Version,a.IsDeleted,a.CreatedOn,a.CreatedByUser,a.ModifiedOn,a.ModifiedByUser,a.DeletedOn,a.DeletedByUser,a.[Identity],a.AccessLevel,a.IsForRole
 from bcms_root.AccessRules as a  where a.Id= @AccessRuleId
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetAllVersionsMediaDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[spGetAllVersionsMediaDetails]
@MediaId  uniqueidentifier,
@OrginalId uniqueidentifier

AS
Begin
declare @flag int


if exists(select mi.Id from bcms_media.MediaImages as mi where mi.Id =@MediaId or mi.Id=@OrginalId)
Begin

select * from  bcms_media.Medias as m inner join bcms_media.MediaImages as mi on  m.Id =mi.Id 
inner join bcms_media.MediaFiles as mf on m.Id = mf.Id where m.Id=@MediaId or m.OriginalId =@OrginalId

End

else if exists(select mf.Id from bcms_media.MediaFiles as mf where mf.Id =@MediaId or mf.Id=@OrginalId)
Begin


select * from bcms_media.Medias as m inner join bcms_media.MediaFiles as mf on  m.Id =mf.Id where m.Id=@MediaId or m.OriginalId =@OrginalId 
End
End
GO
/****** Object:  StoredProcedure [dbo].[spGetAuthorDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetAuthorDetails]
@AuthorId nvarchar(200)

As
Begin
   select a.Version,a.IsDeleted,a.CreatedOn,a.CreatedByUser,a.ModifiedOn,a.ModifiedByUser,a.DeletedOn,a.DeletedByUser,a.Name,a.ImageId,a.Description from bcms_blog.Authors a where a.Id =@AuthorId 

 
End 

GO
/****** Object:  StoredProcedure [dbo].[spGetAuthorId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetAuthorId]
@Name nvarchar(200)

As
Begin
   select bcms_blog.Authors.Id from bcms_blog.Authors where Name=@Name and IsDeleted<>1

 
End 
GO
/****** Object:  StoredProcedure [dbo].[spGetAvailableFor]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetAvailableFor]
@CategoryTreeId uniqueidentifier


As
Begin

 
 select c.Id,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.CategorizableItemId
 from bcms_root.CategoryTreeCategorizableItems as c  where c.CategoryTreeId= @CategoryTreeId and c.IsDeleted <>1
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetBlogCategorizableItem]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetBlogCategorizableItem]


As
Begin

 select c.Id,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedByUser,c.ModifiedOn,c.DeletedOn,c.DeletedByUser,c.Name
 from bcms_root.CategorizableItems as c 

End 
GO
/****** Object:  StoredProcedure [dbo].[spGetBlogCategoryDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetBlogCategoryDetails]
@CategoryId uniqueidentifier


As
Begin

 
 select c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name,c.ParentCategoryId,c.DisplayOrder,c.Macro,c.CategoryTreeId
 from bcms_root.Categories as c  where c.Id= @CategoryId and c.IsDeleted <>1
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetBlogLayoutId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetBlogLayoutId]
@RegionId uniqueidentifier


As
Begin

 select Top 1 l.LayoutId
 from bcms_root.LayoutRegions as l where l.RegionId = @RegionId order by l.CreatedOn
 
End 

GO
/****** Object:  StoredProcedure [dbo].[spGetBlogPageCategoryDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetBlogPageCategoryDetails]
@PageId uniqueidentifier


As
Begin

 
 select p.Id,p.Version,p.IsDeleted,p.CreatedOn,p.CreatedByUser,p.ModifiedOn,p.ModifiedByUser,p.DeletedOn,p.DeletedByUser,p.CategoryId
 from bcms_pages.PageCategories as p  where p.PageId= @PageId
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetBlogPostDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetBlogPostDetails]
@PageId uniqueidentifier,
@AuthorId uniqueidentifier

As
Begin

 select b.Id,b.ActivationDate,b.ExpirationDate,p.Description,p.ImageId,p.CustomCss,p.CustomJS,p.UseCanonicalUrl,p.UseNoFollow,p.UseNoIndex,p.CategoryId,p.SecondaryImageId,p.FeaturedImageId,p.IsArchived,rp.Version,rp.PageUrl,rp.Title,rp.LayoutId,rp.MetaTitle,rp.MetaKeywords,rp.MetaDescription,rp.Status,rp.PageUrlHash,rp.MasterPageId,rp.IsMasterPage,rp.LanguageId,rp.LanguageGroupIdentifier,rp.ForceAccessProtocol
 from bcms_blog.BlogPosts as b
 inner join bcms_pages.Pages as p on b.Id =p.id 
 inner join bcms_root.Pages as rp on b.Id =rp.Id where b.Id= @PageId
 select a.Name,a.Description,a.ImageId,a.Version from bcms_blog.Authors as a where a.Id=@AuthorId


End 
GO
/****** Object:  StoredProcedure [dbo].[spGetBlogRegionId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetBlogRegionId]
@RegionIdentifier varchar(200)

As
Begin

 select r.Id 
 from bcms_root.Regions as r where r.RegionIdentifier = @RegionIdentifier 
 
End 
GO
/****** Object:  StoredProcedure [dbo].[spGetCategoriesforCategoryTree]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetCategoriesforCategoryTree]
@CategoryTreeId uniqueidentifier


As
Begin

 
 select c.Id,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name,c.ParentCategoryId,c.DisplayOrder,c.Macro
 from bcms_root.Categories as c  where c.CategoryTreeId= @CategoryTreeId and c.IsDeleted <>1
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetCategorizableItemDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetCategorizableItemDetails]
@CategorizableItemId uniqueidentifier


As
Begin

 
 select c.Id,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name
 from bcms_root.CategorizableItems as c  where c.Id= @CategorizableItemId and c.IsDeleted <>1
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetCategoryDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetCategoryDetails]
@CategoryId uniqueidentifier


As
Begin

 
 select c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name,c.ParentCategoryId,c.DisplayOrder,c.Macro,c.CategoryTreeId
 from bcms_root.Categories as c  where c.Id= @CategoryId and c.IsDeleted <>1
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetCategoryTreeDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetCategoryTreeDetails]
@CategoryTreeId uniqueidentifier


As
Begin

 
 select c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Title,c.Macro
 from bcms_root.CategoryTrees as c  where c.Id= @CategoryTreeId and c.IsDeleted <>1
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetContentDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetContentDetails]
@PageId uniqueidentifier


As
Begin

 select c.Id,c.Name,c.IsDeleted,c.OriginalId,c.Status,c.Version,c.PreviewUrl,c.CreatedByUser,c.CreatedOn,c.DeletedByUser,c.DeletedOn,c.ModifiedByUser,c.ModifiedOn,c.PublishedOn,c.PublishedByUser,h.ActivationDate,h.ExpirationDate,h.CustomCss,h.UseCustomCss,h.CustomJs,h.UseCustomJs,h.Html,h.EditInSourceMode,h.OriginalText,h.ContentTextMode
 from bcms_root.Contents as c
 inner join bcms_pages.HtmlContents as h on c.Id =h.Id 
 where c.Id= (select Top 1 pc.ContentId from bcms_root.PageContents as pc where pc.PageId= @PageId order by pc.CreatedOn) 
 


End 
GO
/****** Object:  StoredProcedure [dbo].[spGetContentDetailsForLayout]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetContentDetailsForLayout]
@ContentId uniqueidentifier


As
Begin

 select c.Id,c.Name,c.IsDeleted,c.OriginalId,c.Status,c.Version,c.PreviewUrl,c.CreatedByUser,c.CreatedOn,c.DeletedByUser,c.DeletedOn,c.ModifiedByUser,c.ModifiedOn,c.PublishedOn,c.PublishedByUser
 from bcms_root.Contents as c
  where c.Id = @ContentId
  
 


End 

GO
/****** Object:  StoredProcedure [dbo].[spGetContentDetailsForPageContents]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetContentDetailsForPageContents]
@ContentId uniqueidentifier


As
Begin
declare @flag int
if exists(select Id from bcms_blog.BlogPostContents where  Id=@ContentId)
Begin
 set @flag =1
 select flag=@flag
 select h.ActivationDate,h.ExpirationDate,h.CustomCss,h.UseCustomCss,h.CustomJs,h.UseCustomJs,h.Html,h.EditInSourceMode,h.OriginalText,h.ContentTextMode,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name,c.PreviewUrl,c.Status,c.PublishedOn,c.PublishedByUser,c.OriginalId
 from bcms_blog.BlogPostContents as b
 inner join  bcms_pages.HtmlContents as h on b.Id =h.id  
 inner join bcms_root.Contents as c on b.Id =c.Id where b.Id= @ContentId 
 
End
else if exists(select Id from bcms_pages.HtmlContents where  Id=@ContentId)
Begin
set @flag =2
select flag=@flag
select h.ActivationDate,h.ExpirationDate,h.CustomCss,h.UseCustomCss,h.CustomJs,h.UseCustomJs,h.Html,h.EditInSourceMode,h.OriginalText,h.ContentTextMode,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name,c.PreviewUrl,c.Status,c.PublishedOn,c.PublishedByUser,c.OriginalId
from bcms_pages.HtmlContents as h inner join 
bcms_root.Contents as c on h.Id =c.Id where h.Id =@ContentId 


End
else if exists(select Id from bcms_pages.ServerControlWidgets where  Id=@ContentId)
Begin
set @flag =3
select flag=@flag
select s.Url,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name,c.PreviewUrl,c.Status,c.PublishedOn,c.PublishedByUser,c.OriginalId
from bcms_pages.ServerControlWidgets as s inner join
bcms_root.Contents as c on s.Id =c.Id where s.Id =@ContentId

End

else if exists(select Id from bcms_pages.HtmlContentWidgets where  Id=@ContentId)
Begin
set @flag =4
select flag=@flag
select h.CustomCss,h.UseCustomCss,h.CustomJs,h.UseCustomJs,h.Html,h.UseHtml,h.EditInSourceMode,c.Version,c.IsDeleted,c.CreatedOn,c.CreatedByUser,c.ModifiedOn,c.ModifiedByUser,c.DeletedOn,c.DeletedByUser,c.Name,c.PreviewUrl,c.Status,c.PublishedOn,c.PublishedByUser,c.OriginalId
from bcms_pages.HtmlContentWidgets as h inner join
bcms_root.Contents as c on h.Id =c.Id where h.Id =@ContentId

End


End 


GO
/****** Object:  StoredProcedure [dbo].[spGetHistoryDetailsId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[spGetHistoryDetailsId]
@MediaId  uniqueidentifier

AS
Begin

select mi.Id from bcms_media.Medias as mi where mi.OriginalId =@MediaId

End

GO
/****** Object:  StoredProcedure [dbo].[spGetImageInfo]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Proc [dbo].[spGetImageInfo]
@ImageId  uniqueidentifier

AS
Begin
	select Id,Caption from bcms_media.MediaImages where Id = @ImageId
End
GO
/****** Object:  StoredProcedure [dbo].[spGetLanguageDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetLanguageDetails]
@LanguageId uniqueidentifier


As
Begin

 select l.Version,l.IsDeleted,l.CreatedOn,l.CreatedByUser,l.ModifiedOn,l.ModifiedByUser,l.DeletedOn,l.DeletedByUser,l.Name,l.Code
 from bcms_root.Languages as l 
 where l.Id =@LanguageId

End 
GO
/****** Object:  StoredProcedure [dbo].[spGetLayoutDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetLayoutDetails]
@LayoutId uniqueidentifier


As
Begin

 select l.Version,l.IsDeleted,l.CreatedOn,l.CreatedByUser,l.ModifiedOn,l.ModifiedByUser,l.DeletedOn,l.DeletedByUser,l.Name, l.LayoutPath,l.ModuleId,l.PreviewUrl
 from bcms_root.Layouts as l where l.Id =@LayoutId
 
End 
GO
/****** Object:  StoredProcedure [dbo].[spGetLayoutRegionDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetLayoutRegionDetails]
@RegionId uniqueidentifier


As
Begin

 select l.Id,l.Version,l.IsDeleted,l.CreatedOn,l.CreatedByUser,l.ModifiedOn,l.ModifiedByUser,l.DeletedOn,l.DeletedByUser,l.Description,l.LayoutId
 from bcms_root.LayoutRegions as l where l.RegionId =@RegionId and l.IsDeleted<>1

End 
GO
/****** Object:  StoredProcedure [dbo].[spGetMediaCategoriesDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetMediaCategoriesDetails]
@MediaId uniqueidentifier


As
Begin

 
 select mc.Id,mc.Version,mc.IsDeleted,mc.CreatedOn,mc.CreatedByUser,mc.ModifiedOn,mc.ModifiedByUser,mc.DeletedOn,mc.DeletedByUser,mc.CategoryId
 from bcms_media.MediaCategories as mc  where mc.MediaId= @MediaId and mc.IsDeleted <>1
 
 
End
GO
/****** Object:  StoredProcedure [dbo].[spGetMinBlogPostDate]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetMinBlogPostDate]

As
Begin


--select * from bcms_users.Authors

SELECT TOP 1 CreatedOn
FROM bcms_root.Pages
where CreatedByUser <> 'Better Cms' and IsDeleted <> 1
ORDER BY CreatedOn Asc


	
End 

GO
/****** Object:  StoredProcedure [dbo].[spGetPageContentDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetPageContentDetails]
@PageId uniqueidentifier,
@ContentId uniqueidentifier

As
Begin

 select pc.Id,pc.Version,pc.IsDeleted,pc.CreatedOn,pc.CreatedByUser,pc.ModifiedByUser,pc.ModifiedOn,pc.DeletedOn,pc.DeletedByUser,pc.RegionId,pc.[Order]
 from bcms_root.PageContents as pc where pc.PageId =@PageId and pc.ContentId =@ContentId


End 
GO
/****** Object:  StoredProcedure [dbo].[spGetPageContentDetailswithContentId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetPageContentDetailswithContentId]
@ContentId uniqueidentifier

As
Begin

 select pc.Id,pc.Version,pc.IsDeleted,pc.CreatedOn,pc.CreatedByUser,pc.ModifiedByUser,pc.ModifiedOn,pc.DeletedOn,pc.DeletedByUser,pc.PageId,pc.RegionId,pc.[Order],pc.ParentPageContentId
 from bcms_root.PageContents as pc where pc.ContentId =@ContentId


End 
GO
/****** Object:  StoredProcedure [dbo].[spGetPagesForLayout]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetPagesForLayout]
@PageId uniqueidentifier


As
Begin
declare @flag int
if exists(select Id from bcms_blog.BlogPosts where  Id=@PageId)
Begin
 set @flag =1
 select flag=@flag
 select b.Id,b.ActivationDate,b.ExpirationDate,p.Description,p.ImageId,p.CustomCss,p.CustomJS,p.UseCanonicalUrl,p.UseNoFollow,p.UseNoIndex,p.CategoryId,p.SecondaryImageId,p.FeaturedImageId,p.IsArchived,rp.Version,rp.IsDeleted,rp.CreatedOn,rp.CreatedByUser,rp.ModifiedOn,rp.ModifiedByUser,rp.DeletedOn,rp.DeletedByUser,rp.PageUrl,rp.Title,rp.LayoutId,rp.MetaTitle,rp.MetaKeywords,rp.MetaDescription,rp.Status,rp.PageUrlHash,rp.MasterPageId,rp.IsMasterPage,rp.LanguageId,rp.LanguageGroupIdentifier,rp.ForceAccessProtocol
 from bcms_blog.BlogPosts as b
 inner join bcms_pages.Pages as p on b.Id =p.id  
 inner join bcms_root.Pages as rp on b.Id =rp.Id where b.Id= @PageId 
 
End
else if exists(select Id from bcms_pages.Pages where  Id=@PageId)
Begin
set @flag =2
select flag=@flag
select p.Description,p.ImageId,p.CustomCss,p.CustomJS,p.UseCanonicalUrl,p.UseNoFollow,p.UseNoIndex,p.CategoryId,p.SecondaryImageId,p.SecondaryImageId,p.FeaturedImageId,p.IsArchived,rp.Version,rp.IsDeleted,rp.CreatedOn,rp.CreatedByUser,rp.ModifiedOn,rp.ModifiedByUser,rp.DeletedOn,rp.DeletedByUser,rp.PageUrl,rp.Title,rp.LayoutId,rp.PublishedOn,rp.MetaTitle,rp.MetaKeywords,rp.MetaDescription,rp.Status,rp.PageUrlHash,rp.MasterPageId,rp.IsMasterPage,rp.LanguageId,rp.LanguageGroupIdentifier,rp.ForceAccessProtocol
from bcms_pages.Pages as p inner join 
bcms_root.Pages as rp on p.Id =rp.Id where p.Id =@PageId 


End
else if exists(select Id from bcms_root.Pages where  Id=@PageId)
Begin
set @flag =3
select flag=@flag
select rp.Version,rp.IsDeleted,rp.CreatedOn,rp.CreatedByUser,rp.ModifiedOn,rp.ModifiedByUser,rp.DeletedOn,rp.DeletedByUser,rp.PageUrl,rp.Title,rp.LayoutId,rp.PublishedOn,rp.MetaTitle,rp.MetaKeywords,rp.MetaDescription,rp.Status,rp.PageUrlHash,rp.MasterPageId,rp.IsMasterPage,rp.LanguageId,rp.LanguageGroupIdentifier,rp.ForceAccessProtocol
from bcms_root.Pages as rp where rp.Id =@PageId 

End


End 


GO
/****** Object:  StoredProcedure [dbo].[spGetPagesIds]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetPagesIds]
@LayoutId uniqueidentifier


As
Begin

 select rp.Id 
 from bcms_root.Pages as rp  where rp.LayoutId= @LayoutId and rp.IsDeleted <>1
 
End

GO
/****** Object:  StoredProcedure [dbo].[spGetRegionDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetRegionDetails]
@RegionId uniqueidentifier


As
Begin

 select r.Version,r.IsDeleted,r.CreatedOn,r.CreatedByUser,r.ModifiedOn,r.ModifiedByUser,r.DeletedOn, r.DeletedByUser,r.RegionIdentifier
 from bcms_root.Regions as r where r.Id = @RegionId
 
End 
GO
/****** Object:  StoredProcedure [dbo].[spGetRoleId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE proc [dbo].[spGetRoleId]
@RoleName varchar(200)


As
Begin

	select id from bcms_users.Roles where Name=@RoleName and IsDeleted<>'1'

End


GO
/****** Object:  StoredProcedure [dbo].[spGetRolesById]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE  proc [dbo].[spGetRolesById]
@UserId uniqueidentifier


As
Begin

	select  RoleId from BetterCmsTestsDataSet.bcms_users.UserRoles where UserId=@UserId

End


GO
/****** Object:  StoredProcedure [dbo].[spGetUserByScocialId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetUserByScocialId]
@SocialId varchar(50)
as
Begin

       Select * From bcms_users.UserSocial us join bcms_users.Users usr on us.UserId = usr.Id 
       where us.SocialId = @SocialId
End
GO
/****** Object:  StoredProcedure [dbo].[spGetUserIdIfvalid]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE Proc [dbo].[spGetUserIdIfvalid]
@UseName  Nvarchar(255)

AS
Begin
	select Id,Password,Salt from bcms_users.Users where UserName=@UseName
End

GO
/****** Object:  StoredProcedure [dbo].[spGetUserInfo]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[spGetUserInfo]
@UseId  uniqueidentifier

AS
Begin
	SELECT bcms_users.Users.Id, bcms_users.Users.UserName, bcms_users.Users.FirstName,bcms_users.Users.LastName,bcms_users.Users.Email,bcms_users.Users.Password,bcms_users.Users.Salt,bcms_users.Users.ImageId
    
FROM bcms_users.Users right join bcms_users.UserRoles
 on bcms_users.Users.id=bcms_users.UserRoles.UserId  
 where bcms_users.Users.id=@UseId  
 GROUP BY bcms_users.Users.Id, bcms_users.Users.UserName, bcms_users.Users.FirstName,bcms_users.Users.LastName,bcms_users.Users.Email,bcms_users.Users.Password,bcms_users.Users.Salt,bcms_users.Users.ImageId
End
GO
/****** Object:  StoredProcedure [dbo].[spGetUserInfoandroles]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [dbo].[spGetUserInfoandroles]
@UserId  uniqueidentifier

AS
Begin
	SELECT bcms_users.Users.Id, bcms_users.Users.UserName, bcms_users.Users.FirstName,bcms_users.Users.LastName,bcms_users.Users.Email,bcms_users.Users.Password,bcms_users.Users.Salt,bcms_users.Users.ImageId
    
FROM bcms_users.Users
 where bcms_users.Users.id=@UserId  
 select bcms_users.UserRoles.RoleId from bcms_users.UserRoles where bcms_users.UserRoles.UserId=@UserId
 End
GO
/****** Object:  StoredProcedure [dbo].[spGetUserNameByEmail]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Proc [dbo].[spGetUserNameByEmail]
@Email Nvarchar(255)
AS
Begin
	select UserName from bcms_users.Users where Email=@Email
End

GO
/****** Object:  StoredProcedure [dbo].[spMediaDeleteQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spMediaDeleteQuery]
@Id uniqueidentifier,
@Flag int
As
Begin
	if (@Flag='1')
		begin
			Update bcms_media.MediaCategories  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where MediaId=@Id 
			select 1 as result
		end
	if (@Flag='2')
		begin 
			Update bcms_root.AccessRules  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where Id=@Id 
			select 1 as result
		end
    if(@Flag='3')
		begin
			Update bcms_media.MediaTags  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where MediaId=@Id 
			select 1 as result
		end
	 if(@Flag='4')
		begin
			Update bcms_media.Medias  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where Id=@Id 
			select 1 as result
		end

End
GO
/****** Object:  StoredProcedure [dbo].[spMediaGetMedia]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[spMediaGetMedia]
@ImageId  uniqueidentifier

AS
Begin
	select Title,IsArchived,Type,ContentType,OriginalId,PublishedOn,Description from BetterCmsTestsDataSet.bcms_media.Medias where Id=@ImageId

select TagId from bcms_media.MediaTags where MediaId= @ImageId

select CategoryId from BetterCmsTestsDataSet.bcms_media.MediaCategories where MediaId=@ImageId
 End
GO
/****** Object:  StoredProcedure [dbo].[spMediaGetMediaDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[spMediaGetMediaDetails]
@MediaId  uniqueidentifier

AS
Begin
declare @flag int


if exists(select mi.Id from bcms_media.MediaImages as mi where mi.Id =@MediaId)
Begin
set @flag =1
select flag=@flag
select * from bcms_media.MediaImages as mi inner join bcms_media.Medias as m on  mi.Id =m.Id 
inner join bcms_media.MediaFiles as mf on mi.Id = mf.Id where mi.Id=@MediaId

End

else if exists(select mf.Id from bcms_media.MediaFiles as mf where mf.Id =@MediaId)
Begin
set @flag =2
select flag=@flag
select * from bcms_media.MediaFiles as mf inner join bcms_media.Medias as m on  mf.Id =m.Id where mf.Id=@MediaId
End
End
GO
/****** Object:  StoredProcedure [dbo].[spMediaGetVersionCount]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[spMediaGetVersionCount]
@ImageId  uniqueidentifier

AS
Begin
select count(m.Id) from bcms_media.Medias as m where m.OriginalId =@ImageId

End
GO
/****** Object:  StoredProcedure [dbo].[spMediaMoveToTrash]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spMediaMoveToTrash]
@id uniqueidentifier,
@IsMovedToTrash bit

As
Begin
	if exists(select id from bcms_media.Medias where Id = @id)
	begin
	update bcms_media.MediaFiles
	set IsMovedToTrash=@IsMovedToTrash
	where bcms_media.MediaFiles.Id=@id
	select 1 as result
	end
	else
	begin
	select 0 as result
	end
	
End


GO
/****** Object:  StoredProcedure [dbo].[spMediaSaveFile]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spMediaSaveFile]
@id uniqueidentifier,
@Version int,
@IsDeleted bit,
@Title nvarchar(200),
@Type int,

@ContentType int,
@IsArchived bit,
@Description nvarchar(2000),
@OriginalFileName varchar(200),
@OriginalFileExtension varchar(200),
@FileUri nvarchar(2000),
@PublicUrl nvarchar(850),
@Size bigint,
@IsTemporary bit,
@IsUploaded bit,
@IsCanceled bit,
@IsMovedToTrash bit,
@NextTryToMoveToTrash datetime

As
Begin



    declare @_id uniqueidentifier 
    set @_id =NEWID()

	begin
	Insert into bcms_media.Medias(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,Title,Type,ContentType,IsArchived,PublishedOn,Description) 
	values(@_id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate() ,'admin',@Title,@Type,@ContentType,@IsArchived,getutcdate(),@Description) 
	
	end	

	if exists(select id from bcms_media.Medias where Id = @_id)
	begin
	Insert into bcms_media.MediaFiles(Id,OriginalFileName,OriginalFileExtension,FileUri,PublicUrl,Size,IsTemporary,IsUploaded,IsCanceled,IsMovedToTrash,NextTryToMoveToTrash) 
	values(@_id,@OriginalFileName,@OriginalFileExtension,@FileUri,@PublicUrl ,@Size ,@IsTemporary,'1',@IsCanceled,@IsMovedToTrash,@NextTryToMoveToTrash) 
	select @_id
	end
	else
	begin
	select 0 as result
	end
	
End

GO
/****** Object:  StoredProcedure [dbo].[spMediaSaveImage]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spMediaSaveImage]
@id uniqueidentifier,
@Version int,
@IsDeleted bit,
@Title nvarchar(200),
@Type int,

@ContentType int,
@IsArchived bit,
@Description nvarchar(2000),
@OriginalFileName varchar(200),
@OriginalFileExtension varchar(200),
@FileUri nvarchar(2000),
@PublicUrl nvarchar(850),
@Size bigint,
@IsTemporary bit,
@IsUploaded bit,
@IsCanceled bit,
@IsMovedToTrash bit,
@NextTryToMoveToTrash datetime,
@Caption nvarchar(2000),
@ImageAlign int,
@Width int,
@Height int,
@CropCoordX1 int,
@CropCoordY1 int,
@CropCoordX2 int,
@CropCoordY2 int,
@OriginalWidth int,
@OriginalHeight int,
@OriginalSize bigint,
@OriginalUri nvarchar(2000),
@PublicOriginallUrl nvarchar(850),
@IsOriginalUploaded bit,
@ThumbnailWidth int,
@ThumbnailHeight int,
@ThumbnailSize bigint,
@ThumbnailUri nvarchar(2000),
@PublicThumbnailUrl nvarchar(850),
@IsThumbnailUploaded bit,
@Flag int
As
Begin


--select * from bcms_users.Roles
if(@Flag='1')
begin 
    declare @_id uniqueidentifier 
    set @_id =NEWID()
	if(@Flag='1')
	begin
		Insert into bcms_media.Medias(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,Title,Type,ContentType,IsArchived,PublishedOn,Description) 
	values(@_id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate() ,'admin',@Title,@Type,@ContentType,@IsArchived,getutcdate(),@Description) 
	
	end
	if(@Flag='1')
	begin
	Insert into bcms_media.MediaFiles(Id,OriginalFileName,OriginalFileExtension,FileUri,PublicUrl,Size,IsTemporary,IsUploaded,IsCanceled,IsMovedToTrash,NextTryToMoveToTrash) 
	values(@_id,@OriginalFileName,@OriginalFileExtension,@FileUri,@PublicUrl ,@Size ,@IsTemporary,'1',@IsCanceled,@IsMovedToTrash,@NextTryToMoveToTrash) 
	
	
	end

	if(@Flag='1')
	begin
	Insert into bcms_media.MediaImages(Id,Caption,ImageAlign,Width,Height ,CropCoordX1 ,CropCoordY1,CropCoordX2,CropCoordY2,OriginalWidth,OriginalHeight,OriginalSize,OriginalUri,
	PublicOriginallUrl,IsOriginalUploaded,ThumbnailWidth,ThumbnailHeight,ThumbnailSize,ThumbnailUri,PublicThumbnailUrl,IsThumbnailUploaded) 
	values(@_id,@Caption,3,@Width,@Height ,@CropCoordX1 ,@CropCoordY1,@CropCoordX2,@CropCoordY2,@OriginalWidth,@OriginalHeight,@OriginalSize,@OriginalUri,
	@PublicOriginallUrl,@IsOriginalUploaded,@ThumbnailWidth,@ThumbnailHeight,@ThumbnailSize,@ThumbnailUri,@PublicThumbnailUrl,@IsThumbnailUploaded) 
	select @_id
	end
	
	
	end
	if(@flag='2')
	begin
	if exists(select id from bcms_media.MediaFiles where Id = @_id)
	begin
	update bcms_media.MediaImages
	set Caption=@Caption,ImageAlign=@ImageAlign,Width=@Width,Height=@Height,CropCoordX1=@CropCoordX1,CropCoordY1=@CropCoordY1,CropCoordX2=@CropCoordX2,CropCoordY2=@CropCoordY2,
	OriginalWidth=@OriginalWidth, OriginalHeight=@OriginalHeight,OriginalSize=@OriginalSize,OriginalUri=@OriginalUri,PublicOriginallUrl=@PublicOriginallUrl,IsOriginalUploaded=@IsOriginalUploaded,
	ThumbnailWidth=@ThumbnailWidth,ThumbnailHeight=@ThumbnailHeight,ThumbnailSize=@ThumbnailSize, ThumbnailUri=@ThumbnailUri,PublicThumbnailUrl=@PublicThumbnailUrl, IsThumbnailUploaded=@IsThumbnailUploaded
	Where id=@id
	select @_id
	end
	end
End



GO
/****** Object:  StoredProcedure [dbo].[spMediaSaveMedia]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spMediaSaveMedia]
@id uniqueidentifier,
@Version int,
@IsDeleted bit,
@Title nvarchar(200),
@Type int,

@ContentType int,
@IsArchived bit,
@Description nvarchar(2000)


As
Begin



    declare @_id uniqueidentifier 
    set @_id =NEWID()

	begin
	Insert into bcms_media.Medias(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,Title,Type,ContentType,IsArchived,PublishedOn,Description) 
	values(@_id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate() ,'admin',@Title,@Type,@ContentType,@IsArchived,getutcdate(),@Description) 
	select 1 as result
	end	

	
End



GO
/****** Object:  StoredProcedure [dbo].[spMediaUpdateFile]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spMediaUpdateFile]
@id uniqueidentifier,
@Version int,
@IsDeleted bit,
@Title nvarchar(200),
@Type int,

@ContentType int,
@IsArchived bit,
@Description nvarchar(2000),
@OriginalFileName varchar(200),
@OriginalFileExtension varchar(200),
@FileUri nvarchar(2000),
@PublicUrl nvarchar(850),
@Size bigint,
@IsTemporary bit,
@IsUploaded bit,
@IsCanceled bit,
@IsMovedToTrash bit,
@NextTryToMoveToTrash datetime

As
Begin

	if exists(select id from bcms_media.Medias where Id = @id)
	begin
	update bcms_media.MediaFiles set OriginalFileName= @OriginalFileName,OriginalFileExtension=@OriginalFileExtension,FileUri=@FileUri,
	PublicUrl=@PublicUrl,Size=@Size, IsTemporary=@IsTemporary,IsUploaded='1',IsCanceled=@IsCanceled,IsMovedToTrash=@IsMovedToTrash,
	NextTryToMoveToTrash=@NextTryToMoveToTrash
    where Id=@id
	select 1 as result
	end
	else
	begin
	select 0 as result
	end
	
End



GO
/****** Object:  StoredProcedure [dbo].[spRootcategoryFetchQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootcategoryFetchQuery]
@Id uniqueidentifier,
@Flag int
As
Begin
	    if(@Flag=1)
		begin
			select CategoryId from bcms_pages.PageCategories where PageId=@Id
			
		end
	    if(@Flag=2)
		begin
		select Name from bcms_root.Categories where Id=@Id
		end
End


GO
/****** Object:  StoredProcedure [dbo].[spRootCategoryIdQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootCategoryIdQuery]
@key nvarchar(200),
@Flag int

As
Begin
	
		
	    if(@Flag=1)
		begin
			select CategoryTreeId from bcms_root.CategoryTreeCategorizableItems as ct join bcms_root.CategorizableItems as c
			 on ct.CategorizableItemId =c.Id
			  where c.Name=@key
			
		end
	
	

End
GO
/****** Object:  StoredProcedure [dbo].[spRootCategoryQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootCategoryQuery]
@Id uniqueidentifier

As
Begin
	
		begin
			select Id,Version,IsDeleted,Name,DisplayOrder,Macro,CategoryTreeId from bcms_root.Categories Where Id=@Id and IsDeleted!=1
			
		end
	

End
GO
/****** Object:  StoredProcedure [dbo].[spRootContentHistoryList]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootContentHistoryList]
@Id nvarchar(200)


As
Begin
		begin
			select bcms_root.Contents.Id,bcms_root.Contents.Name,PreviewUrl,PublishedOn,PublishedByUser,Version,IsDeleted,ModifiedOn,ModifiedByUser,bcms_root.ContentStatuses.Name as Status, OriginalId from bcms_root.Contents join bcms_root.ContentStatuses
			 on Status =  bcms_root.ContentStatuses.Id
			  where bcms_root.Contents.OriginalId=@Id and IsDeleted!=1
			
		end
End

GO
/****** Object:  StoredProcedure [dbo].[spRootContentQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootContentQuery]
@Id nvarchar(200)


As
Begin
		begin
			select bcms_root.Contents.Name,PreviewUrl,PublishedOn,PublishedByUser,Version,IsDeleted,ModifiedOn,ModifiedByUser,bcms_root.ContentStatuses.Name as Status, OriginalId from bcms_root.Contents join bcms_root.ContentStatuses
			 on Status =  bcms_root.ContentStatuses.Id
			  where bcms_root.Contents.Id=@Id
			
		end
End
GO
/****** Object:  StoredProcedure [dbo].[spRootDeleteQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootDeleteQuery]
@Id uniqueidentifier,
@Flag int
As
Begin
	if (@Flag='1')
		begin
			Update bcms_pages.PageCategories  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where Id=@Id 
			select 1 as result
		end
	if (@Flag='2')
		begin
			Update bcms_root.Categories  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where Id=@Id 
			select 1 as result
		end
	if (@Flag='3')
		begin
			Update bcms_root.ChildContents  set IsDeleted = 1 ,DeletedOn=GETUTCDATE(),DeletedByUser='admin' Where Id=@Id 
			select 1 as result
		end	
End

GO
/****** Object:  StoredProcedure [dbo].[spRootFetchChildContentQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchChildContentQuery]
@Id nvarchar(200)


As
Begin
		begin
			select ChildContentId from  bcms_root.ChildContents
			
			  where ParentContentId=@Id and IsDeleted!=1
			
		end
End

GO
/****** Object:  StoredProcedure [dbo].[spRootFetchContentoptionQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchContentoptionQuery]
@Id nvarchar(200)


As
Begin
		begin
			select bcms_root.ContentOptions.Id, [Key],Type,DefaultValue,IsDeletable,CustomOptionId from bcms_root.ContentOptions 
			  where bcms_root.ContentOptions.ContentId=@Id and IsDeleted!=1
			
		end
End



GO
/****** Object:  StoredProcedure [dbo].[spRootFetchFetchContentRegionQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchFetchContentRegionQuery]
@Id nvarchar(200)


As
Begin
		begin
			select bcms_root.Regions.Id,bcms_root.Regions.RegionIdentifier from  bcms_root.Regions join bcms_root.ContentRegions 
			on bcms_root.Regions.Id = bcms_root.ContentRegions.RegionId
     		  where bcms_root.ContentRegions.ContentId=@Id
			
		end
End


GO
/****** Object:  StoredProcedure [dbo].[spRootFetchLanguage]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchLanguage]


As
Begin
	
		begin
			select Name,Code from bcms_root.Languages where IsDeleted!=1
			
		end
	

End



GO
/****** Object:  StoredProcedure [dbo].[spRootFetchLayoutRegionListQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchLayoutRegionListQuery]
@Id nvarchar(200)


As
Begin
		begin
			select bcms_root.LayoutRegions.LayoutId from  bcms_root.LayoutRegions
     		  where bcms_root.LayoutRegions.RegionId=@Id
			
		end
End

GO
/****** Object:  StoredProcedure [dbo].[spRootFetchLayoutRegionQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchLayoutRegionQuery]
@Id nvarchar(200)


As
Begin
		begin
			select Id,Version,IsDeleted,ModifiedOn,ModifiedByUser,CreatedOn,CreatedByUser,LayoutId,RegionId,Description from  bcms_root.LayoutRegions
     		  where bcms_root.LayoutRegions.RegionId=@Id
			
		end
End



GO
/****** Object:  StoredProcedure [dbo].[spRootFetchPageContentlistQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchPageContentlistQuery]
@Id nvarchar(200)


As
Begin
		begin
			select Id from  bcms_root.PageContents
     		  where bcms_root.PageContents.RegionId=@Id
			
		end
End


GO
/****** Object:  StoredProcedure [dbo].[spRootFetchPageContentQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchPageContentQuery]
@Id nvarchar(200)


As
Begin
		begin
			select Id,Version,IsDeleted,ModifiedOn,ModifiedByUser,CreatedOn,CreatedByUser,PageId,ContentId,[Order],RegionId,ParentPageContentId from  bcms_root.PageContents
     		  where bcms_root.PageContents.Id=@Id
			
		end
End


GO
/****** Object:  StoredProcedure [dbo].[spRootFetchQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchQuery]
@Id uniqueidentifier

As
Begin
	
		begin
			select Name from bcms_root.Categories Where Id=@Id 
			
		end
	

End
GO
/****** Object:  StoredProcedure [dbo].[spRootFetchRegionsQuery]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootFetchRegionsQuery]
@Id nvarchar(200)


As
Begin
		begin
			select Version,ModifiedOn, ModifiedByUser, RegionIdentifier from bcms_root.Regions 
			  where bcms_root.Regions.Id=@Id and IsDeleted!=1
			
		end
End


GO
/****** Object:  StoredProcedure [dbo].[spRootGetCategorylistbyId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spRootGetCategorylistbyId]
@Id nvarchar(200),
@Flag int

As
Begin
	
		
	    if(@Flag=1)
		begin
			select Id,Name,ParentCategoryId from bcms_root.Categories where CategoryTreeId=@Id and IsDeleted!=1
			
		end
	
	

End

GO
/****** Object:  StoredProcedure [dbo].[spRootRegionPageContentDetails]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spRootRegionPageContentDetails]
@RegionId uniqueidentifier


As
Begin

 select p.Id,p.Version,p.IsDeleted,p.CreatedOn,p.CreatedByUser,p.ModifiedOn,p.ModifiedByUser,p.DeletedOn,p.DeletedByUser,p.PageId,p.ContentId,p.[Order]
 from bcms_root.PageContents as p where p.RegionId =@RegionId and p.IsDeleted <> 1

End 
GO
/****** Object:  StoredProcedure [dbo].[spRootSaveOrUpdateContent]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spRootSaveOrUpdateContent]
@id uniqueidentifier,
@Version int,
@IsDeleted bit,
@Name nvarchar(200),
@PreviewUrl int,
@ModifiedByUser nvarchar(200),
@Status int,
@PublishedByUser nvarchar(2000),
@OriginalId uniqueidentifier


As
Begin


     if exists(select id from bcms_media.Medias where Id = @id)
	begin
	update bcms_root.Contents 
	set Version=@Version, IsDeleted=@IsDeleted,Name=@Name,PreviewUrl=@PreviewUrl,ModifiedOn=getutcdate(),ModifiedByUser=@ModifiedByUser,
	Status=@Status,PublishedOn=getutcdate(),PublishedByUser=@PublishedByUser,OriginalId=@OriginalId
	where Id=@id
	select @id as ID
	end
	
	else
	begin
    declare @_id uniqueidentifier 
    set @_id =NEWID()

	begin
	Insert into bcms_root.Contents(Id,Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn,ModifiedByUser,Name,PreviewUrl,Status,PublishedOn,PublishedByUser,OriginalId) 
	values(@_id,@Version,@IsDeleted,getutcdate(),'admin',getutcdate() ,'admin',@Name,@PreviewUrl,@Status,getutcdate(),@PublishedByUser,@OriginalId) 
	select @_id as ID
	end
	end	

	
	
	
End


 
GO
/****** Object:  StoredProcedure [dbo].[spRootSelectContentbyId]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spRootSelectContentbyId]
@Id uniqueidentifier

As
Begin


declare @flag int
if exists(select Id from bcms_blog.BlogPostContents where  Id=@Id)
Begin
 set @flag =1
 select flag=@flag
 select Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn, ModifiedByUser,Name,PreviewUrl,Status,PublishedOn,PublishedByUser
      ,OriginalId,ActivationDate,ExpirationDate,CustomCss,UseCustomCss,CustomJs,UseCustomJs,Html,EditInSourceMode,OriginalText,ContentTextMode from bcms_root.Contents join bcms_pages.HtmlContents on bcms_root.Contents.Id = bcms_pages.HtmlContents.Id
	  where bcms_root.Contents.Id= @Id
 
End
else if exists(select Id from bcms_pages.HtmlContentWidgets where  Id=@Id)
Begin
set @flag =2
select flag=@flag
select Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn, ModifiedByUser,Name,PreviewUrl,Status,PublishedOn,PublishedByUser
      ,OriginalId,CustomCss,UseCustomCss,CustomJs,UseCustomJs,Html,UseHtml,EditInSourceMode
	   from bcms_root.Contents join bcms_pages.HtmlContentWidgets on bcms_root.Contents.Id = bcms_pages.HtmlContentWidgets.Id
	  where bcms_root.Contents.Id=@Id


End
else if exists(select Id from bcms_pages.HtmlContents where  Id=@Id)
Begin
set @flag =3
select flag=@flag
select Version,IsDeleted,CreatedOn,CreatedByUser,ModifiedOn, ModifiedByUser,Name,PreviewUrl,Status,PublishedOn,PublishedByUser
      ,OriginalId,ActivationDate,ExpirationDate,CustomCss,UseCustomCss,CustomJs,UseCustomJs,Html,EditInSourceMode,OriginalText,ContentTextMode
	   from bcms_root.Contents join bcms_pages.HtmlContents on bcms_root.Contents.Id = bcms_pages.HtmlContents.Id
	  where bcms_root.Contents.Id=@Id

End

End
GO
/****** Object:  StoredProcedure [dbo].[spSaveUser]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spSaveUser]
@id uniqueidentifier,
@version int,
@UserName varchar(50),
@FirstName varchar(50),
@LastName varchar(50),
@Email varchar(50),
@Password varchar(50),
@ImageId uniqueidentifier,
@Salt varchar(50)

As
Begin


--select * from bcms_users.Roles

    declare @_id uniqueidentifier 
    set @_id =NEWID()
	Insert into bcms_users.Users(Id,UserName,FirstName,LastName,Email,Password,ImageId,Salt,version,ModifiedByUser,ModifiedOn,CreatedByUser,CreatedOn) 
	values(@_id,@UserName,@FirstName,@LastName,@Email,@Password,@ImageId,@Salt,@version,'admin',getutcdate(),'admin',getutcdate())
	Select @_id
End
GO
/****** Object:  StoredProcedure [dbo].[spSaveUserwithoutImage]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spSaveUserwithoutImage]
@id uniqueidentifier,
@version int,
@UserName varchar(50),
@FirstName varchar(50),
@LastName varchar(50),
@Email varchar(50),
@Password varchar(50),
@Salt varchar(50)

As
Begin


--select * from bcms_users.Roles

    declare @_id uniqueidentifier 
    set @_id =NEWID()
	Insert into bcms_users.Users(Id,UserName,FirstName,LastName,Email,Password,Salt,version,ModifiedByUser,ModifiedOn,CreatedByUser,CreatedOn) 
	values(@_id,@UserName,@FirstName,@LastName,@Email,@Password,@Salt,@version,'admin',getutcdate(),'admin',getutcdate())
	Select @_id
End
GO
/****** Object:  StoredProcedure [dbo].[spUpdateAuthor]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spUpdateAuthor]
@Id uniqueidentifier,
@Version int,
@Name nvarchar(100),
@ImageId uniqueidentifier,
@Description nvarchar(500)
As
Begin
	Begin Try
		Update bcms_blog.Authors  set Version=@Version,ModifiedOn=getutcdate(),ModifiedByUser='admin',Name=@Name,ImageId=@ImageId,Description=@Description Where Id=@Id 
		select 1 as result
	End Try
	Begin Catch
		select 0 as result
	End Catch
		
End

GO
/****** Object:  StoredProcedure [dbo].[spUpdateUserwithImage]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spUpdateUserwithImage]
@id uniqueidentifier,
@version int,
@UserName varchar(50),
@FirstName varchar(50),
@LastName varchar(50),
@Email varchar(50),
@ImageId uniqueidentifier


As
Begin


--select * from bcms_users.Roles

    update bcms_users.Users
	set UserName=@UserName ,FirstName=@FirstName, LastName=@LastName,Email=@Email,ImageId=@ImageId
	where bcms_users.Users.Id=@id
	select @id
End
GO
/****** Object:  StoredProcedure [dbo].[spUpdateUserwithoutImage]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spUpdateUserwithoutImage]
@id uniqueidentifier,
@version int,
@UserName varchar(50),
@FirstName varchar(50),
@LastName varchar(50),
@Email varchar(50)


As
Begin


--select * from bcms_users.Roles

    update bcms_users.Users
	set UserName=@UserName ,FirstName=@FirstName, LastName=@LastName,Email=@Email
	where bcms_users.Users.Id=@id
	select @id
End
GO
/****** Object:  StoredProcedure [dbo].[spValidateRoleName]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Proc [dbo].[spValidateRoleName]
@Id uniqueidentifier,
@Name Nvarchar(255)
AS
Begin
	if exists(select id from bcms_users.Roles  where Name = @Name and id <>@Id)
	Begin
		select 0 as result
	End
	else
	Begin
		select 1 as result
	End

End


GO
/****** Object:  StoredProcedure [dbo].[spValidateUserEmail]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE Proc [dbo].[spValidateUserEmail]
@Id uniqueidentifier,
@Email Nvarchar(255)
AS
Begin
	if exists(select id from bcms_users.Users  where Email = @Email and id <>@Id)
	Begin
		select 0 as result
	End
	
	else
	Begin
		select 1 as result
	End

End

GO
/****** Object:  StoredProcedure [dbo].[spValidateUserName]    Script Date: 6/15/2017 11:23:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE Proc [dbo].[spValidateUserName]
@Id uniqueidentifier,
@Username Nvarchar(255)
AS
Begin
	if exists(select id from bcms_users.Users  where UserName = @Username and id <>@Id)
	Begin
		select 0 as result
	End
	else
	Begin
		select 1 as result
	End

End
GO
